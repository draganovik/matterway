using Asp.Versioning;
using AutoMapper;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Ordering.Api.Features.OrderItems.Contracts;
using Ordering.Api.Features.OrderItems.Data;
using Ordering.Api.Features.OrderItems.Domain;

namespace Ordering.Api.Features.OrderItems.Endpoints;

public class GetOrderItemById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Orders/{id:guid}/Items/{itemId:guid}", Handler)
            .WithName("GetOrderItemById").WithSummary("Get Order Item by id.")
            .WithTags(nameof(OrderItem))
            .Produces<OrderItemBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<OrderItemBaseResponse>, NotFound>>
        Handler(Guid id, Guid itemId, IOrderItemRepository orderItemRepository, IMapper mapper)
    {
        var entity = await orderItemRepository.GetById(id, itemId);
        return entity is not null
            ? TypedResults.Ok(mapper.Map<OrderItemBaseResponse>(entity))
            : TypedResults.NotFound();
    }
}