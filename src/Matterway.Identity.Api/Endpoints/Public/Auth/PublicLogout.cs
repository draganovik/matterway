using System.Security.Claims;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Infrastructure.Services.AuthToken;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Endpoints.Public.Auth;

public class PublicLogout : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Public, "auth/logout", Handler)
            .WithName("PublicLogout")
            .WithSummary("[public] Invalidate current session tokens.")
            .WithTags("Auth")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<NoContent, UnauthorizedHttpResult>> Handler(
        HttpContext context,
        UserManager<SystemUser> userManager)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var parsedId)) return TypedResults.Unauthorized();

        var user = await userManager.FindByIdAsync(parsedId.ToString());
        if (user is null) return TypedResults.Unauthorized();

        await userManager.RemoveAuthenticationTokenAsync(user,
            JwtTokenService.RefreshTokenIdProvider,
            JwtTokenService.RefreshTokenIdName);
        await userManager.UpdateSecurityStampAsync(user);
        return TypedResults.NoContent();
    }
}