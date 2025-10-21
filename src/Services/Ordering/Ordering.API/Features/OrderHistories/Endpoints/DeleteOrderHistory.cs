using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Ordering.API.Features.OrderHistories.Data;
using Ordering.API.Features.OrderHistories.Domain;
using Shared.Enums;
using Shared.Iterfaces;

namespace Ordering.API.Features.OrderHistories.Endpoints;

public class DeleteOrderHistory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("OrderHistories/{id:guid}", Handler)
            .WithName("DeleteOrderHistory").WithSummary("Delete Order History by id.")
            .WithTags(nameof(OrderHistory))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound>>
        Handler(Guid id, IOrderHistoryRepository orderHistoryRepository)
    {
        var isDeleted = await orderHistoryRepository.Delete(id);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}