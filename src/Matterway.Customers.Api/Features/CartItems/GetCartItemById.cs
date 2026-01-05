using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CartItemEntity;

namespace Matterway.Customers.Api.Features.CartItems;

public class GetCartItemById(ICartItemRepository cartItemRepository) :
    EndpointWithoutRequest<GetCartItemById.CartItemResponse>
{
    public override void Configure()
    {
        Get("/customers/{id:guid}/cartitems/{productId:guid}");
        Version(1);
        Options(options =>
        {
            options.WithName("GetCartItemById")
                .WithSummary("Get CartItem by id.")
                .WithTags(nameof(CartItem))
                .Produces<CartItemResponse>()
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy => policy.RequireRole(
                    nameof(ERequestClaimsRole.Admin),
                    nameof(ERequestClaimsRole.Manager)));
        });
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");
        var productId = Route<Guid>("productId");

        var entity = await cartItemRepository.GetBy(id, productId, cancellationToken);
        if (entity == null)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        await Send.OkAsync(MapToResponse(entity), cancellationToken);
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