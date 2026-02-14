using System.Security.Claims;

namespace Matterway.ServiceDefaults.Authorization;

public enum PermissionLevel
{
    Observer = 0,
    Operator = 1,
    Administrator = 2
}

public static class PermissionClaims
{
    public const string ClaimType = "perm";
    private const string EmployeeRoleName = "Employee";

    public static string Format(string service, PermissionLevel level)
    {
        if (string.IsNullOrWhiteSpace(service))
            throw new ArgumentException("Service name is required.", nameof(service));

        return $"{NormalizeService(service)}:{NormalizeLevel(level)}";
    }

    public static bool TryParse(string value, out string service, out PermissionLevel level)
    {
        service = string.Empty;
        level = default;

        if (string.IsNullOrWhiteSpace(value)) return false;

        var parts = value.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) return false;

        var parsedService = parts[0].Trim();
        var levelPart = parts[1].Trim();
        if (string.IsNullOrWhiteSpace(parsedService) || string.IsNullOrWhiteSpace(levelPart)) return false;

        if (!Enum.TryParse(levelPart, true, out PermissionLevel parsedLevel)) return false;

        service = NormalizeService(parsedService);
        level = parsedLevel;
        return true;
    }

    public static bool HasPermission(ClaimsPrincipal? user, string service, PermissionLevel minimumLevel)
    {
        if (user is null) return false;
        if (!IsEmployee(user)) return false;

        var normalizedService = NormalizeService(service);
        foreach (var claim in user.FindAll(ClaimType))
        {
            if (!TryParse(claim.Value, out var claimService, out var claimLevel)) continue;
            if (!string.Equals(claimService, normalizedService, StringComparison.OrdinalIgnoreCase)) continue;
            if (claimLevel >= minimumLevel) return true;
        }

        return false;
    }

    public static bool IsEmployee(ClaimsPrincipal user)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        return string.Equals(role, EmployeeRoleName, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeService(string service)
    {
        return service.Trim().ToLowerInvariant();
    }

    private static string NormalizeLevel(PermissionLevel level)
    {
        return level.ToString().ToLowerInvariant();
    }
}