using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Providers.Persistence.CartItemEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.CartItems;

public class GetCartItemById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers/{customerId:guid}/CartItems/{productId:guid}", Handler)
            .WithName("GetCartItemById").WithSummary("Get CartItem by id.")
            .WithTags(nameof(CartItem))
            .Produces<CartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestRole.Admin),
                nameof(ERequestRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<CartItemResponse>, NotFound>> Handler(
        Guid customerId,
        Guid productId,
        ICartItemRepository cartItemRepository,
        CancellationToken cancellationToken)
    {
        var entity = await cartItemRepository.GetBy(customerId, productId, cancellationToken);
        if (entity == null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(entity));
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
        [Range(typeof(decimal), "0.01", "2147483647")]
        public decimal? UnitPrice { get; init; }
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