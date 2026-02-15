using Asp.Versioning;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Self.Orders;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Sales.Api.Features.Admin.Orders;

public class AdminGetOrderById : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "orders/{orderId:guid}", Handler)
            .WithName("AdminGetOrderById").WithSummary("[admin] Get Order by id")
            .WithTags(nameof(Order))
            .Produces<SelfCreateOrder.OrderResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsObserver(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<SelfCreateOrder.OrderResponse>, NotFound>> Handler(
        Guid orderId,
        IOrderRepository orderRepository,
        CancellationToken cancellationToken)
    {
        var entity = await orderRepository.GetById(orderId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();
        return TypedResults.Ok(SelfCreateOrder.MapToResponse(entity));
    }
}