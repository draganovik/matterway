using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Matterway.ServiceDefaults.Authorization;

namespace Matterway.ServiceDefaults.Api;

public sealed record RequestIdentityOptions
{
    public required string ServiceName { get; init; }
    public string CustomerRoleName { get; init; } = "Customer";
    public Func<string?, bool>? IsCustomerRole { get; init; }
}

public static class RequestIdentity
{
    private static RequestIdentityOptions? _configuredOptions;

    public static void Configure(RequestIdentityOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ServiceName);

        _configuredOptions = options;
    }

    public static Guid? GetIdentifier(ClaimsPrincipal? principal)
    {
        if (principal is null) return null;

        var subject = GetSubjectValue(principal);
        return Guid.TryParse(subject, out var identifier) ? identifier : null;
    }

    public static bool IsCustomer(ClaimsPrincipal? principal)
    {
        var roleValue = principal?.FindFirstValue(ClaimTypes.Role);
        var options = GetOptions();

        if (options.IsCustomerRole is not null)
            return options.IsCustomerRole(roleValue);

        return string.Equals(roleValue, options.CustomerRoleName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool AsObserver(ClaimsPrincipal? principal)
    {
        return HasPermission(principal, PermissionLevel.Observer);
    }

    public static bool AsOperator(ClaimsPrincipal? principal)
    {
        return HasPermission(principal, PermissionLevel.Operator);
    }

    public static bool AsAdministrator(ClaimsPrincipal? principal)
    {
        return HasPermission(principal, PermissionLevel.Administrator);
    }

    public static bool CanManageOwnedResource(ClaimsPrincipal? principal, Guid ownerId)
    {
        var requesterId = GetIdentifier(principal);
        if (requesterId is null) return false;

        return !IsCustomer(principal) || requesterId.Value == ownerId;
    }

    private static bool HasPermission(ClaimsPrincipal? principal, PermissionLevel level)
    {
        return principal is not null &&
               PermissionClaims.HasPermission(principal, GetOptions().ServiceName, level);
    }

    private static RequestIdentityOptions GetOptions()
    {
        return _configuredOptions
               ?? throw new InvalidOperationException(
                   "RequestIdentity is not configured. Call RequestIdentity.Configure(...) during startup.");
    }

    private static string? GetSubjectValue(ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(subject)) return subject;

        return principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
    }
}