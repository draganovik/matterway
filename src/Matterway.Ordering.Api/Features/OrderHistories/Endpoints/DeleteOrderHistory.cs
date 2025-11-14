using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Common.Enums;
using Matterway.Ordering.Api.Features.OrderHistories.Data;
using Matterway.Ordering.Api.Features.OrderHistories.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Ordering.Api.Features.OrderHistories.Endpoints;

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