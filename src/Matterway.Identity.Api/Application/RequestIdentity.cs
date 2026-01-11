using System.Security.Claims;
using Matterway.Identity.Api.Domain;

namespace Matterway.Identity.Api.Application;

public readonly record struct RequestClaims(Guid SystemUserId, EIdentityRole Role);

public static class RequestIdentity
{
    public static bool TryGet(ClaimsPrincipal principal, out RequestClaims requestClaims)
    {
        requestClaims = default;

        if (!Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var systemUserId))
            return false;

        if (!Enum.TryParse(principal.FindFirstValue(ClaimTypes.Role), out EIdentityRole role))
            return false;

        requestClaims = new RequestClaims(systemUserId, role);
        return true;
    }
}