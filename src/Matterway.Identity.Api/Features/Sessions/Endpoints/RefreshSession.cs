using Asp.Versioning;
using AutoMapper;
using Matterway.Catalog.Api.Application;
using Matterway.Identity.Api.Features.Sessions.Contracts;
using Matterway.Identity.Api.Features.Sessions.Data;
using Matterway.Identity.Api.Features.Sessions.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Identity.Api.Features.Sessions.Endpoints;

public class RefreshSession : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Sessions/refresh", Handler)
            .WithName("RefreshSession").WithSummary("Refresh an expired session.")
            .WithTags("Sessions")
            .Produces<SessionBaseResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<SessionBaseResponse>, BadRequest<ProblemDetails>, UnauthorizedHttpResult>>
        Handler(
            SessionRefreshRequest request,
            ISessionRepository sessionRepository,
            IConfiguration configuration,
            IMapper mapper)
    {
        var session = await sessionRepository.GetByRefreshToken(request.RefreshToken!);
        if (session is null || session.Expires > DateTime.UtcNow || session.SystemUser is null)
            return TypedResults.BadRequest(CreateProblemDetails("Token did not expire or refresh token was invalid."));

        if (session.IsRefreshExpired())
        {
            await sessionRepository.DeleteByRefreshToken(request.RefreshToken!);
            return TypedResults.BadRequest(CreateProblemDetails("Refresh token has expired."));
        }

        var (token, tokenDescriptor) = JwtOperations.Generate(session.SystemUser, configuration);
        var (refreshToken, refreshDescriptor) = JwtOperations.Generate(session.SystemUser, configuration, true);

        session.Token = token;
        session.RefreshToken = refreshToken;
        session.Created = tokenDescriptor.IssuedAt ?? DateTime.UtcNow;
        session.Expires = tokenDescriptor.Expires ?? DateTime.UtcNow;
        session.RefreshExpires = refreshDescriptor.Expires ?? DateTime.UtcNow;

        var refreshedSession = await sessionRepository.Refresh(session);
        if (refreshedSession is null) return TypedResults.Unauthorized();

        return TypedResults.Ok(mapper.Map<SessionBaseResponse>(refreshedSession));
    }

    private static ProblemDetails CreateProblemDetails(string detail)
    {
        return new ProblemDetails
        {
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = detail
        };
    }
}