using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Matterway.ServiceDefaults.Authorization;

namespace Matterway.Customers.Api.Application;

public static class RequestIdentity
{
    public const string ServiceName = "customers";
    private const string CustomerRoleName = "Customer";

    public static Guid? GetIdentifier(ClaimsPrincipal? principal)
    {
        if (principal is null) return null;

        var subject = GetSubjectValue(principal);
        return Guid.TryParse(subject, out var identifier) ? identifier : null;
    }

    public static bool IsCustomer(ClaimsPrincipal? principal)
    {
        var roleValue = principal?.FindFirstValue(ClaimTypes.Role);
        return string.Equals(roleValue, CustomerRoleName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool AsObserver(ClaimsPrincipal? principal)
    {
        return PermissionClaims.HasPermission(principal, ServiceName, PermissionLevel.Observer);
    }

    public static bool AsOperator(ClaimsPrincipal? principal)
    {
        return PermissionClaims.HasPermission(principal, ServiceName, PermissionLevel.Operator);
    }

    public static bool CanManageOwnedResource(ClaimsPrincipal? principal, Guid ownerId)
    {
        var requesterId = GetIdentifier(principal);
        if (requesterId is null) return false;

        return !IsCustomer(principal) || requesterId.Value == ownerId;
    }

    private static string? GetSubjectValue(ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(subject)) return subject;

        return principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
    }
}