using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using Identity.API.Features.Sessions.Contracts;
using Identity.API.Features.Sessions.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Iterfaces;

namespace Identity.API.Features.Sessions.Endpoints;

public class IntrospectSession : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Sessions/introspect", Handler)
            .WithName("IntrospectSession").WithSummary("Introspect session token.")
            .WithTags("Sessions")
            .Produces<SessionBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<SessionBaseResponse>, UnauthorizedHttpResult>> Handler(
        HttpContext context,
        ISessionRepository sessionRepository,
        IMapper mapper)
    {
        var identity = context.User.Identity as ClaimsIdentity;

        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
        {
            return TypedResults.Unauthorized();
        }

        var token = await context.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            return TypedResults.Unauthorized();
        }

        var currentSession = await sessionRepository.GetByToken(token);
        if (currentSession is null || currentSession.SystemUserId != systemUserId)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok(mapper.Map<SessionBaseResponse>(currentSession));
    }
}