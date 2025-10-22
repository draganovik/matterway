using Asp.Versioning;
using AutoMapper;
using Identity.API.Features.Shared;
using Identity.API.Features.Sessions.Contracts;
using Identity.API.Features.Sessions.Data;
using Identity.API.Features.Sessions.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;
using Common.Infrastructure.ModelTemplates;

namespace Identity.API.Features.Sessions.Endpoints;

public class QuerySessions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Sessions", Handler)
            .WithName("QuerySessions").WithSummary("Query sessions.")
            .WithTags("Sessions")
            .Produces<PaginationResponse<SessionBaseResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<SessionBaseResponse>>, NoContent, BadRequest<ProblemDetails>
            , ValidationProblem>>
        Handler([AsParameters] PagingQueryParams pagingQuery,
            HttpContext httpContext,
            ISessionRepository sessionRepository,
            IMapper mapper)
    {
        var total = await sessionRepository.GetTotalEntities();
        var entities = await sessionRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value);
        var baseUri = ResourceUrlHelper.CreateBaseUri(httpContext, "Sessions");

        var paginationResponse = new PaginationResponse<SessionBaseResponse>(
            total,
            pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<SessionBaseResponse>>(entities).ToList(),
            baseUri);

        return entities is IEnumerable<Session> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}