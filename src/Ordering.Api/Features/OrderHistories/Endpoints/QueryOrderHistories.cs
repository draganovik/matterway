using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Http;
using Matterway.Common.Pagination;
using Microsoft.AspNetCore.Http.HttpResults;
using Ordering.Api.Features.OrderHistories.Contracts;
using Ordering.Api.Features.OrderHistories.Data;
using Ordering.Api.Features.OrderHistories.Domain;

namespace Ordering.Api.Features.OrderHistories.Endpoints;

public class QueryOrderHistories : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("OrderHistories", Handler)
            .WithName("QueryOrderHistories").WithSummary("Query Order Histories.")
            .WithTags(nameof(OrderHistory))
            .Produces<PaginationResponse<OrderHistoryBaseResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<OrderHistoryBaseResponse>>, NoContent, ValidationProblem>>
        Handler(
            [AsParameters]
            PagingQueryParams pagingQuery,
            HttpContext httpContext,
            IOrderHistoryRepository orderHistoryRepository,
            IMapper mapper)
    {
        var total = await orderHistoryRepository.GetTotalEntities();
        var entities = await orderHistoryRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value);
        if (!entities.Any()) return TypedResults.NoContent();

        var baseUri = ApiResourceUriBuilder.BuildAbsoluteUri(httpContext, "OrderHistories");
        var paginationResponse = new PaginationResponse<OrderHistoryBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<OrderHistoryBaseResponse>>(entities).ToList(), baseUri);

        return TypedResults.Ok(paginationResponse);
    }
}