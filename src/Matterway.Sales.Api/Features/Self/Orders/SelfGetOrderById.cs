using Asp.Versioning;
using Matterway.ServiceDefaults.Api;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Sales.Api.Features.Self.Orders;

public class SelfGetOrderById : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Self, "orders/{orderId:guid}", Handler)
            .WithName("SelfGetOrderById").WithSummary("[self] Get own Order by id.")
            .WithTags(nameof(Order))
            .Produces<SelfCreateOrder.OrderResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<SelfCreateOrder.OrderResponse>, NotFound, ForbidHttpResult>> Handler(
        Guid orderId,
        HttpContext httpContext,
        IOrderRepository orderRepository,
        CancellationToken cancellationToken)
    {
        var customerId = RequestIdentity.GetIdentifier(httpContext.User);
        if (customerId is null) return TypedResults.Forbid();

        var entity = await orderRepository.GetById(orderId, cancellationToken);
        if (entity is null || entity.CustomerId != customerId) return TypedResults.NotFound();

        return TypedResults.Ok(SelfCreateOrder.MapToResponse(entity));
    }
}