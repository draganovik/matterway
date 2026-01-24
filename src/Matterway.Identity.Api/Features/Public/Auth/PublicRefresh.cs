using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Identity.Api.Application;
using Matterway.Identity.Api.Infrastructure.Services.AuthToken;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Identity.Api.Features.Public.Auth;

public class PublicRefresh : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("public/auth/refresh", Handler)
            .WithName("PublicRefresh")
            .WithSummary("Refresh access token using a refresh token.")
            .WithTags("Auth")
            .Produces<RefreshResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .AllowAnonymous()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RefreshResponse>, UnauthorizedHttpResult>> Handler(
        AuthRefreshRequest request,
        ITokenService tokenService)
    {
        var tokens = await tokenService.RefreshTokenAsync(request.RefreshToken!);
        return tokens is null ? TypedResults.Unauthorized() : TypedResults.Ok(MapToResponse(tokens));
    }

    public class AuthRefreshRequest
    {
        [Required(ErrorMessage = "RefreshToken is required.")]
        public string? RefreshToken { get; set; }
    }

    public class RefreshResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public DateTime RefreshExpires { get; set; }
    }

    private static RefreshResponse MapToResponse(AuthTokenServiceResponse entity)
    {
        return new RefreshResponse
        {
            Token = entity.Token,
            RefreshToken = entity.RefreshToken,
            Created = entity.Created,
            Expires = entity.Expires,
            RefreshExpires = entity.RefreshExpires
        };
    }
}