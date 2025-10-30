using Asp.Versioning;
using Matterway.Common.Abstractions;
using Identity.Api.Features.Sessions.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace Identity.Api.Features.Sessions.Endpoints;

public class RevokeSession : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Sessions/revoke", Handler)
            .WithName("RevokeSession").WithSummary("Revoke the current session.")
            .WithTags("Sessions")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> Handler(
        HttpContext context,
        ISessionRepository sessionRepository)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out _))
        {
            return TypedResults.Unauthorized();
        }

        var token = await context.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            return TypedResults.Unauthorized();
        }

        var isDeleted = await sessionRepository.DeleteByToken(token);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}