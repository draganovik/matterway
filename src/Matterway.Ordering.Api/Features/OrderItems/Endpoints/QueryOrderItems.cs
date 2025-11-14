using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Pagination;
using Matterway.Ordering.Api.Features.OrderItems.Contracts;
using Matterway.Ordering.Api.Features.OrderItems.Data;
using Matterway.Ordering.Api.Features.OrderItems.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Ordering.Api.Features.OrderItems.Endpoints;

public class QueryOrderItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Orders/Items", Handler)
            .WithName("QueryOrderItems").WithSummary("Query Order Items.")
            .WithTags(nameof(OrderItem))
            .Produces<PaginationResponse<OrderItemBaseResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<OrderItemBaseResponse>>, NoContent, ValidationProblem>>
        Handler(
            [AsParameters]
            PagingQueryParams pagingQuery,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            IOrderItemRepository orderItemRepository,
            IMapper mapper)
    {
        var total = await orderItemRepository.GetTotalEntities();
        var entities = await orderItemRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value);
        if (!entities.Any()) return TypedResults.NoContent();

        var baseUri = linkGenerator.GetPathByName(httpContext, "QueryOrderItems", null);
        ArgumentNullException.ThrowIfNull(baseUri);
        var paginationResponse = new PaginationResponse<OrderItemBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<OrderItemBaseResponse>>(entities).ToList(), new Uri(baseUri));

        return TypedResults.Ok(paginationResponse);
    }
}