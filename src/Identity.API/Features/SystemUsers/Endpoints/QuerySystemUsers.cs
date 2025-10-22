using Asp.Versioning;
using AutoMapper;
using Identity.API.Features.Shared;
using Identity.API.Features.SystemUsers.Contracts;
using Identity.API.Features.SystemUsers.Data;
using Identity.API.Features.SystemUsers.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Iterfaces;
using SharedProject.ModelTemplates;

namespace Identity.API.Features.SystemUsers.Endpoints;

public class QuerySystemUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("SystemUsers", Handler)
            .WithName("QuerySystemUsers").WithSummary("Query system users.")
            .WithTags("SystemUsers")
            .Produces<PaginationResponse<SystemUserBaseResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<SystemUserBaseResponse>>, NoContent,
            BadRequest<ProblemDetails>, ValidationProblem>>
        Handler([AsParameters] PagingQueryParams pagingQuery,
            HttpContext httpContext,
            ISystemUserRepository systemUserRepository,
            IMapper mapper)
    {
        var total = await systemUserRepository.GetTotalEntities();
        var entities = await systemUserRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value);
        var baseUri = ResourceUrlHelper.CreateBaseUri(httpContext, "SystemUsers");

        var paginationResponse = new PaginationResponse<SystemUserBaseResponse>(
            total,
            pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<SystemUserBaseResponse>>(entities).ToList(),
            baseUri);

        return entities is IEnumerable<SystemUser> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}