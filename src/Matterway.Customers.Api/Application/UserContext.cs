using System.Security.Claims;
using Matterway.Customers.Api.Domain;

namespace Matterway.Customers.Api.Application;

public readonly record struct AuthenticatedUser(Guid SystemUserId, ERequestClaimsRole Role);

public static class UserContext
{
    public static bool TryGet(ClaimsPrincipal user, out AuthenticatedUser authenticatedUser)
    {
        authenticatedUser = default;

        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var systemUserId))
            return false;

        if (!Enum.TryParse(user.FindFirstValue(ClaimTypes.Role), out ERequestClaimsRole role))
            return false;

        authenticatedUser = new AuthenticatedUser(systemUserId, role);
        return true;
    }
}