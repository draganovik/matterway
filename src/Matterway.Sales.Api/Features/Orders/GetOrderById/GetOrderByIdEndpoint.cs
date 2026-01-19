using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Orders.CreateOrder;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Sales.Api.Features.Orders.GetOrderById;

public class GetOrderByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Orders/{orderId:guid}", Handler)
            .WithName("GetOrderById").WithSummary("Get Order by id.")
            .WithTags(nameof(Order))
            .Produces<OrderResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<OrderResponse>, NotFound>> Handler(
        Guid orderId,
        GetOrderByIdService getOrderByIdService,
        CancellationToken cancellationToken)
    {
        var entity = await getOrderByIdService.GetByIdAsync(orderId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();
        return TypedResults.Ok(CreateOrderMapper.MapToResponse(entity));
    }
}