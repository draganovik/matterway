using System.ComponentModel.DataAnnotations;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Infrastructure.Services.AuthToken;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Endpoints.Public.Auth;

public class PublicLogin : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Public, "auth/login", Handler)
            .WithName("PublicLogin")
            .WithSummary("[public] Authenticate user and issue tokens.")
            .WithTags("Auth")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .AllowAnonymous()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> Handler(
        AuthLoginRequest request,
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
        return TypedResults.Ok(MapToResponse(tokens));
    }

    public class AuthLoginRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public DateTime RefreshExpires { get; set; }
    }

    private static LoginResponse MapToResponse(AuthTokenServiceResponse entity)
    {
        return new LoginResponse
        {
            Token = entity.Token,
            RefreshToken = entity.RefreshToken,
            Created = entity.Created,
            Expires = entity.Expires,
            RefreshExpires = entity.RefreshExpires
        };
    }
}