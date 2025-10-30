using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Ordering.Api.Features.Orders.Contracts;
using Ordering.Api.Features.Orders.Data;
using Ordering.Api.Features.Orders.Domain;

namespace Ordering.Api.Features.Orders.Endpoints;

public class GetOrderById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Orders/{id:guid}", Handler)
            .WithName("GetOrderById").WithSummary("Get Order by id.")
            .WithTags(nameof(Order))
            .Produces<OrderBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<OrderBaseResponse>, NotFound>>
        Handler(Guid id, IOrderRepository orderRepository, IMapper mapper)
    {
        var entity = await orderRepository.GetById(id);
        return entity is not null
            ? TypedResults.Ok(mapper.Map<OrderBaseResponse>(entity))
            : TypedResults.NotFound();
    }
}