using System.Security.Claims;
using Asp.Versioning;
using Matterway.Identity.Api.Infrastructure.Services.AuthToken;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Identity.Api.Features.System.Auth;

public class SystemIntrospect : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.System, "auth/introspect", Handler)
            .WithName("SystemIntrospect")
            .WithSummary("[system] Introspect access token and return claims")
            .WithTags("Auth")
            .Produces<AuthIntrospectResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static Results<Ok<AuthIntrospectResponse>, UnauthorizedHttpResult> Handler(HttpContext context)
    {
        var principal = context.User;
        if (principal?.Identity?.IsAuthenticated != true) return TypedResults.Unauthorized();

        if (!string.Equals(principal.FindFirstValue(JwtTokenService.TokenUseClaim),
                JwtTokenService.AccessTokenUse, StringComparison.Ordinal))
            return TypedResults.Unauthorized();

        var claims = principal.Claims
            .Where(claim => claim.Type is not JwtTokenService.SecurityStampClaim
                and not JwtTokenService.TokenUseClaim)
            .Select(claim => new AuthClaim
            {
                Type = claim.Type,
                Value = claim.Value
            })
            .ToList();

        return TypedResults.Ok(new AuthIntrospectResponse { Claims = claims });
    }

    public class AuthIntrospectResponse
    {
        public List<AuthClaim> Claims { get; set; } = new();
    }

    public class AuthClaim
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}