using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Matterway.Identity.Api.Domain;
using Matterway.ServiceDefaults.Authorization;

namespace Matterway.Identity.Api.Application;

public static class RequestIdentity
{
    public const string ServiceName = "identity";

    public static Guid? GetIdentifier(ClaimsPrincipal? principal)
    {
        if (principal is null) return null;

        var subject = GetSubjectValue(principal);
        return Guid.TryParse(subject, out var identifier) ? identifier : null;
    }

    public static bool IsCustomer(ClaimsPrincipal? principal)
    {
        var roleValue = principal?.FindFirstValue(ClaimTypes.Role);
        return Enum.TryParse(roleValue, true, out EIdentityRole role) && role == EIdentityRole.Customer;
    }

    public static bool AsOperator(ClaimsPrincipal? principal)
    {
        return principal is not null &&
               PermissionClaims.HasPermission(principal, ServiceName, PermissionLevel.Operator);
    }

    public static bool AsAdministrator(ClaimsPrincipal? principal)
    {
        return principal is not null &&
               PermissionClaims.HasPermission(principal, ServiceName, PermissionLevel.Administrator);
    }

    private static string? GetSubjectValue(ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(subject)) return subject;

        return principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
    }
}