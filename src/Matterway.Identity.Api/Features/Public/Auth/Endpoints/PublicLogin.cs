using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Features.Public.Auth.Contracts;
using Matterway.Identity.Api.Infrastructure.Services.AuthToken;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Features.Public.Auth.Endpoints;

public class PublicLogin : IEndpoint
{
    private const string RouteName = nameof(PublicLogin);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Public, "auth/login", Handler)
            .WithName(RouteName)
            .WithSummary("[public] Authenticate user and issue tokens.")
            .WithTags("Auth")
            .Produces<PublicLoginAuthResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .AllowAnonymous()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<PublicLoginAuthResponse>, UnauthorizedHttpResult>> Handler(
        PublicLoginAuthRequest request,
        UserManager<SystemUser> userManager,
        SignInManager<SystemUser> signInManager,
        ITokenService tokenService)
    {
        var user = await userManager.FindByEmailAsync(request.Email!);
        if (user is null) return TypedResults.Unauthorized();

        var signInResult = await signInManager.CheckPasswordSignInAsync(
            user, request.Password!, true);

        if (!signInResult.Succeeded) return TypedResults.Unauthorized();

        var tokens = await tokenService.CreateTokenPairAsync(user);
        return TypedResults.Ok(ToResponse(tokens));
    }

    private static PublicLoginAuthResponse ToResponse(AuthTokenServiceResponse entity)
    {
        return new PublicLoginAuthResponse
        {
            Token = entity.Token,
            RefreshToken = entity.RefreshToken,
            Created = entity.Created,
            Expires = entity.Expires,
            RefreshExpires = entity.RefreshExpires
        };
    }
}