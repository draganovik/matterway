using System.Security.Claims;
using Matterway.Identity.Api.Features.System.Auth.Contracts;
using Matterway.Identity.Api.Infrastructure.Services.AuthToken;

namespace Matterway.Identity.Api.Features.System.Auth.Endpoints;

public class SystemIntrospect : IEndpoint
{
    private const string RouteName = nameof(SystemIntrospect);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.System, "auth/introspect", Handler)
            .WithName(RouteName)
            .WithSummary("[system] Introspect access token and return claims")
            .WithTags("Auth")
            .Produces<SystemIntrospectAuthResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireSystemAccessKey()
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static Results<Ok<SystemIntrospectAuthResponse>, UnauthorizedHttpResult> Handler(HttpContext context)
    {
        var principal = context.User;
        if (principal?.Identity?.IsAuthenticated != true) return TypedResults.Unauthorized();

        if (!string.Equals(principal.FindFirstValue(JwtTokenService.TokenUseClaim),
                JwtTokenService.AccessTokenUse, StringComparison.Ordinal))
            return TypedResults.Unauthorized();

        var claims = principal.Claims
            .Where(claim => claim.Type is not JwtTokenService.SecurityStampClaim
                and not JwtTokenService.TokenUseClaim)
            .Select(claim => new SystemIntrospectAuthResponse.ClaimResponse
            {
                Type = claim.Type,
                Value = claim.Value
            })
            .ToList();

        return TypedResults.Ok(new SystemIntrospectAuthResponse { Claims = claims });
    }
}
