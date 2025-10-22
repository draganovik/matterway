using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Ordering.Api.Features.Orders.Data;
using Ordering.Api.Features.Orders.Domain;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Abstractions;

namespace Ordering.Api.Features.Orders.Endpoints;

public class DeleteOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Orders/{id:guid}", Handler)
            .WithName("DeleteOrder").WithSummary("Delete Order by id.")
            .WithTags(nameof(Order))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound>> Handler(Guid id, IOrderRepository orderRepository)
    {
        var isDeleted = await orderRepository.Delete(id);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}