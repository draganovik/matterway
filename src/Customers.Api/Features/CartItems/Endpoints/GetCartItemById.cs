using Asp.Versioning;
using AutoMapper;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Enums;
using Customers.Api.Features.CartItems.Contracts;
using Customers.Api.Features.CartItems.Data;
using Customers.Api.Features.CartItems.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Customers.Api.Features.CartItems.Endpoints;

public class GetCartItemById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers/{id:guid}/CartItems/{productId:guid}", Handler)
            .WithName("GetCartItemById").WithSummary("Get CartItem by id.")
            .WithTags(nameof(CartItem))
            .Produces<CartItemBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<CartItemBaseResponse>, NotFound>> Handler(
        Guid id,
        Guid productId,
        ICartItemRepository cartItemRepository,
        IMapper mapper)
    {
        return await cartItemRepository.GetById(id, productId)
            is CartItem value
            ? TypedResults.Ok(mapper.Map<CartItemBaseResponse>(value))
            : TypedResults.NotFound();
    }
}