using System.Security.Claims;
using Matterway.Customers.Api.Domain;

namespace Matterway.Customers.Api.Application;

public readonly record struct RequestClaims(Guid SystemUserId, ERequestRole Role);

public static class RequestIdentity
{
    public static bool TryGet(ClaimsPrincipal principal, out RequestClaims requestClaims)
    {
        requestClaims = default;

        if (!Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var systemUserId))
            return false;

        if (!Enum.TryParse(principal.FindFirstValue(ClaimTypes.Role), out ERequestRole role))
            return false;

        requestClaims = new RequestClaims(systemUserId, role);
        return true;
    }
}