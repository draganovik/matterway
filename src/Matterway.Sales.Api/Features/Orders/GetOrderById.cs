using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Sales.Api.Features.Orders;

public class GetOrderById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Orders/{orderId:guid}", Handler)
            .WithName("GetOrderById").WithSummary("Get Order by id.")
            .WithTags(nameof(Order))
            .Produces<CreateOrder.OrderResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<CreateOrder.OrderResponse>, NotFound>> Handler(
        Guid orderId,
        IOrderRepository orderRepository,
        CancellationToken cancellationToken)
    {
        var entity = await orderRepository.GetById(orderId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();
        return TypedResults.Ok(CreateOrder.MapToResponse(entity));
    }
}