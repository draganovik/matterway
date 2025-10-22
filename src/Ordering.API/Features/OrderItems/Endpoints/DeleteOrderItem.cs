using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Ordering.API.Features.OrderItems.Data;
using Ordering.API.Features.OrderItems.Domain;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Ordering.API.Features.OrderItems.Endpoints;

public class DeleteOrderItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Orders/{id:guid}/Items/{itemId:guid}", Handler)
            .WithName("DeleteOrderItem").WithSummary("Delete Order Item by id.")
            .WithTags(nameof(OrderItem))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound>>
        Handler(Guid id, Guid itemId, IOrderItemRepository orderItemRepository)
    {
        var isDeleted = await orderItemRepository.Delete(id, itemId);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}