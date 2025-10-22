using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Features.OrderItems.Contracts;
using Ordering.API.Features.OrderItems.Data;
using Ordering.API.Features.OrderItems.Domain;
using Ordering.API.Features.Shared;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;
using Common.Infrastructure.ModelTemplates;

namespace Ordering.API.Features.OrderItems.Endpoints;

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
            IOrderItemRepository orderItemRepository,
            IMapper mapper)
    {
        var total = await orderItemRepository.GetTotalEntities();
        var entities = await orderItemRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value);
        if (!entities.Any()) return TypedResults.NoContent();

        var baseUri = ResourceUrlHelper.CreateBaseUri(httpContext, "Orders/Items");
        var paginationResponse = new PaginationResponse<OrderItemBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<OrderItemBaseResponse>>(entities).ToList(), baseUri);

        return TypedResults.Ok(paginationResponse);
    }
}