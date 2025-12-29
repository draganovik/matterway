using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.EntityCartItem;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.CartItems;

public class GetCartItemById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers/{id:guid}/CartItems/{productId:guid}", Handler)
            .WithName("GetCartItemById").WithSummary("Get CartItem by id.")
            .WithTags(nameof(CartItem))
            .Produces<CartItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<CartItemResponse>, NotFound>> Handler(
        Guid id,
        Guid productId,
        ICartItemRepository cartItemRepository)
    {
        return await cartItemRepository.GetById(id, productId)
            is CartItem value
            ? TypedResults.Ok(MapToResponse(value))
            : TypedResults.NotFound();
    }

    public record CartItemResponse
    {
        [Required]
        public Guid CustomerId { get; init; }

        [Required]
        public string? ProductName { get; init; }

        [Required]
        public Guid ProductId { get; init; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public double UnitPrice { get; init; }
    }

    private static CartItemResponse MapToResponse(CartItem entity)
    {
        return new CartItemResponse
        {
            CustomerId = entity.CustomerId,
            ProductId = entity.ProductId,
            ProductName = entity.ProductName,
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice
        };
    }
}