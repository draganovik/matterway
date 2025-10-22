using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Ordering.Api.Features.OrderHistories.Contracts;
using Ordering.Api.Features.OrderHistories.Data;
using Ordering.Api.Features.OrderHistories.Domain;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Ordering.Api.Features.OrderHistories.Endpoints;

public class GetOrderHistoryById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("OrderHistories/{id:guid}", Handler)
            .WithName("GetOrderHistoryById").WithSummary("Get Order History by id.")
            .WithTags(nameof(OrderHistory))
            .Produces<OrderHistoryBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<OrderHistoryBaseResponse>, NotFound>>
        Handler(Guid id, IOrderHistoryRepository orderHistoryRepository, IMapper mapper)
    {
        var entity = await orderHistoryRepository.GetById(id);
        return entity is not null
            ? TypedResults.Ok(mapper.Map<OrderHistoryBaseResponse>(entity))
            : TypedResults.NotFound();
    }
}