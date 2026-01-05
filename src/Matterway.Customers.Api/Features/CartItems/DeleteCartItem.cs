using FastEndpoints;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CartItemEntity;
using Matterway.Customers.Api.Application;

namespace Matterway.Customers.Api.Features.CartItems;

public class DeleteCartItem(ICartItemRepository cartItemRepository) :
    EndpointWithoutRequest<DeleteCartItem.DeleteCartItemResponse>
{
    public override void Configure()
    {
        Delete("/customers/{id:guid}/cartitems/{productId:guid}");
        Version(1);
        Options(options =>
        {
            options.WithName("DeleteCartItem")
                .WithSummary("Delete CartItem.")
                .WithTags(nameof(CartItem))
                .Produces<DeleteCartItemResponse>()
                .Produces(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .RequireAuthorization();
        });
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
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

        var isDeleted = await cartItemRepository.Delete(customerId, productId, cancellationToken);
        if (!isDeleted)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        await Send.OkAsync(new DeleteCartItemResponse
        {
            CustomerId = customerId,
            ProductId = productId
        }, cancellationToken);
    }

    public record DeleteCartItemResponse
    {
        public Guid CustomerId { get; init; }
        public Guid ProductId { get; init; }
        public string Message { get; init; } = "Cart item removed successfully.";
    }
}