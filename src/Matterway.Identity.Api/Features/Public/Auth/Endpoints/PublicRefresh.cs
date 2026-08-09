using Matterway.Identity.Api.Features.Public.Auth.Contracts;
using Matterway.Identity.Api.Infrastructure.Services.AuthToken;

namespace Matterway.Identity.Api.Features.Public.Auth.Endpoints;

public class PublicRefresh : IEndpoint
{
    private const string RouteName = nameof(PublicRefresh);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Public, "auth/refresh", Handler)
            .WithName(RouteName)
            .WithSummary("[public] Refresh access token using a refresh token.")
            .WithTags("Auth")
            .Produces<PublicRefreshAuthResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .AllowAnonymous()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<PublicRefreshAuthResponse>, UnauthorizedHttpResult>> Handler(
        PublicRefreshAuthRequest request,
        ITokenService tokenService)
    {
        var tokens = await tokenService.RefreshTokenAsync(request.RefreshToken!);
        return tokens is null ? TypedResults.Unauthorized() : TypedResults.Ok(ToResponse(tokens));
    }

    private static PublicRefreshAuthResponse ToResponse(AuthTokenServiceResponse entity)
    {
        return new PublicRefreshAuthResponse
        {
            Token = entity.Token,
            RefreshToken = entity.RefreshToken,
            Created = entity.Created,
            Expires = entity.Expires,
            RefreshExpires = entity.RefreshExpires
        };
    }
}