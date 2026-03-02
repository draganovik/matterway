using System.Security.Claims;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.ServiceDefaults;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Domain;

public static class IdentityPermissionAdapter
{
    private static readonly StringComparer ServiceComparer = StringComparer.OrdinalIgnoreCase;

    public static IReadOnlyList<string> KnownServices { get; } = ApiDirectory.All
        .Select(api => api.ServiceName)
        .Where(service => !string.IsNullOrWhiteSpace(service))
        .Select(NormalizeService)
        .Distinct(ServiceComparer)
        .OrderBy(service => service, ServiceComparer)
        .ToArray();

    public static async Task<IdentityResult> EnsureEmployeeObserverDefaultsAsync(
        UserManager<SystemUser> userManager,
        SystemUser user)
    {
        var claims = await userManager.GetClaimsAsync(user);
        var errors = new List<IdentityError>();

        foreach (var service in KnownServices)
        {
            if (claims.Any(claim =>
                    string.Equals(claim.Type, PermissionClaims.ClaimType, StringComparison.Ordinal) &&
                    MatchesService(claim.Value, service)))
                continue;

            var addResult = await userManager.AddClaimAsync(
                user,
                new Claim(PermissionClaims.ClaimType, PermissionClaims.Format(service, PermissionLevel.Observer)));

            if (!addResult.Succeeded)
                errors.AddRange(addResult.Errors);
        }

        return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed([.. errors]);
    }

    public static async Task<IdentityResult> SetServicePermissionAsync(
        UserManager<SystemUser> userManager,
        SystemUser user,
        string service,
        PermissionLevel level)
    {
        if (!TryNormalizeKnownService(service, out var normalizedService))
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Unsupported service '{service}'."
            });

        var errors = new List<IdentityError>();
        var claims = await userManager.GetClaimsAsync(user);
        var serviceClaims = claims
            .Where(claim =>
                string.Equals(claim.Type, PermissionClaims.ClaimType, StringComparison.Ordinal) &&
                MatchesService(claim.Value, normalizedService))
            .ToArray();

        foreach (var existingClaim in serviceClaims)
        {
            var removeResult = await userManager.RemoveClaimAsync(user, existingClaim);
            if (!removeResult.Succeeded)
                errors.AddRange(removeResult.Errors);
        }

        if (errors.Count == 0)
        {
            var claimValue = PermissionClaims.Format(normalizedService, level);
            var addResult = await userManager.AddClaimAsync(user, new Claim(PermissionClaims.ClaimType, claimValue));
            if (!addResult.Succeeded)
                errors.AddRange(addResult.Errors);
        }

        if (errors.Count > 0)
            return IdentityResult.Failed([.. errors]);

        return await EnsureEmployeeObserverDefaultsAsync(userManager, user);
    }

    public static async Task<IReadOnlyList<SystemUserPermission>> GetServicePermissionsAsync(
        UserManager<SystemUser> userManager,
        SystemUser user)
    {
        var claims = await userManager.GetClaimsAsync(user);
        return GetServicePermissions(claims);
    }

    public static IReadOnlyList<SystemUserPermission> GetServicePermissions(IEnumerable<Claim> claims)
    {
        var permissions = new Dictionary<string, PermissionLevel>(ServiceComparer);

        foreach (var claim in claims)
        {
            if (!string.Equals(claim.Type, PermissionClaims.ClaimType, StringComparison.Ordinal))
                continue;

            if (!PermissionClaims.TryParse(claim.Value, out var service, out var level))
                continue;

            if (!TryNormalizeKnownService(service, out var normalizedService))
                continue;

            if (!permissions.TryGetValue(normalizedService, out var existingLevel) ||
                CompareLevel(level, existingLevel) > 0)
                permissions[normalizedService] = level;
        }

        foreach (var service in KnownServices)
            permissions.TryAdd(service, PermissionLevel.Observer);

        return permissions
            .OrderBy(item => item.Key, ServiceComparer)
            .Select(item => new SystemUserPermission(item.Key, item.Value))
            .ToArray();
    }

    public static bool TryNormalizeKnownService(string? service, out string normalizedService)
    {
        normalizedService = string.Empty;
        if (string.IsNullOrWhiteSpace(service)) return false;

        normalizedService = NormalizeService(service);
        return KnownServices.Contains(normalizedService, ServiceComparer);
    }

    private static int CompareLevel(PermissionLevel left, PermissionLevel right)
    {
        static int Weight(PermissionLevel level)
        {
            return level switch
            {
                PermissionLevel.Manager => 3,
                PermissionLevel.Operator => 2,
                _ => 1
            };
        }

        return Weight(left).CompareTo(Weight(right));
    }

    private static bool MatchesService(string claimValue, string normalizedService)
    {
        if (string.IsNullOrWhiteSpace(claimValue)) return false;

        var parts = claimValue.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) return false;

        var claimService = NormalizeService(parts[0]);
        return string.Equals(claimService, normalizedService, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeService(string service)
    {
        return service.Trim().ToLowerInvariant();
    }
}

public sealed record SystemUserPermission(string Service, PermissionLevel Level);