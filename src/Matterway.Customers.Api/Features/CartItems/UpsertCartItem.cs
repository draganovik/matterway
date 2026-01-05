using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Brokers.Catalog;
using Matterway.Customers.Api.Infrastructure.Persistence.CartItemEntity;
using FluentValidation.Results;

namespace Matterway.Customers.Api.Features.CartItems;

public class UpsertCartItem(ICartItemRepository cartItemRepository, ICatalogClient catalogClient) :
    Endpoint<UpsertCartItem.CartItemRequest, UpsertCartItem.CartItemResponse>
{
    public override void Configure()
    {
        Put("/customers/{id:guid}/cartitems/{productId:guid}");
        Version(1);
        Options(options =>
        {
            options.WithName("UpsertCartItemById")
                .WithSummary("Upsert CartItem.")
                .WithTags(nameof(CartItem))
                .Produces<CartItemResponse>()
                .Produces(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .RequireAuthorization();
        });
    }

    public override async Task HandleAsync(CartItemRequest request, CancellationToken cancellationToken)
    {
        if (!UserContext.TryGet(User, out var userContext))
        {
            await Send.ForbiddenAsync(cancellationToken);
            return;
        }

        var customerId = Route<Guid>("id");
        var productId = Route<Guid>("productId");

        if (userContext.Role == ERequestClaimsRole.Customer && customerId != userContext.SystemUserId)
        {
            await Send.ForbiddenAsync(cancellationToken);
            return;
        }

        var product = await catalogClient.GetProductById(productId, cancellationToken);
        if (product is null)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        var resolvedPrice = product.Price ?? product.BasePrice;
        if (resolvedPrice is null)
        {
            var errors = new List<ValidationFailure>
            {
                new(string.Empty, "Product price is unavailable.")
            };
            await HttpContext.Response.SendErrorsAsync(errors, StatusCodes.Status400BadRequest, null,
                cancellationToken);
            return;
        }

        var entity = new CartItem
        {
            CustomerId = customerId,
            ProductId = productId,
            Quantity = request.Quantity,
            ProductName = product.Title,
            UnitPrice = resolvedPrice
        };

        try
        {
            var stored = await cartItemRepository.Upsert(entity, cancellationToken);
            if (stored is null)
            {
                await Send.NotFoundAsync(cancellationToken);
                return;
            }

            await Send.OkAsync(MapToResponse(stored), cancellationToken);
        }
        catch (Exception ex)
        {
            var errors = new List<ValidationFailure>
            {
                new(string.Empty, ex.Message)
            };
            await HttpContext.Response.SendErrorsAsync(errors, StatusCodes.Status400BadRequest, null,
                cancellationToken);
        }
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

    public record CartItemRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }
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