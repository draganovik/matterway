using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Sales.Api.Features.Orders;

public class QueryOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Orders", Handler)
            .WithName("QueryOrders").WithSummary("Query Orders.")
            .WithTags(nameof(Order))
            .Produces<PaginationResponse<CreateOrder.OrderResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<CreateOrder.OrderResponse>>, NoContent, ValidationProblem>>
        Handler([AsParameters] QueryOrdersParameters queryParameters,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            IOrderRepository orderRepository,
            CancellationToken cancellationToken)
    {
        var total = await orderRepository.Count(queryParameters.CustomerId, cancellationToken);
        if (total == 0)
            return TypedResults.NoContent();

        var entities = await orderRepository.Query(
            queryParameters.Page,
            queryParameters.PageSize,
            queryParameters.CustomerId,
            cancellationToken);

        var location = linkGenerator.GetUriByName(
            httpContext,
            "QueryOrders",
            null);

        var results = entities.Select(CreateOrder.MapToResponse).ToList();

        var paginationResponse = PaginationResponse<CreateOrder.OrderResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }

    public sealed record QueryOrdersParameters : PaginationRequestParameters
    {
        public Guid? CustomerId { get; init; }
    }
}