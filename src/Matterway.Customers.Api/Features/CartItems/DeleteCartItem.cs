using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using Matterway.Customers.Api.Infrastructure.Persistence.CartItemEntity;

namespace Matterway.Customers.Api.Features.CartItems;

public class DeleteCartItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Customers/{customerId:guid}/CartItems/{productId:guid}", Handler)
            .WithName("DeleteCartItem").WithSummary("Delete CartItem.")
            .WithTags(nameof(CartItem))
            .Produces<DeleteCartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteCartItemResponse>, NotFound, ForbidHttpResult>> Handler(
        Guid customerId,
        Guid productId,
        HttpContext httpContext,
        ICartItemRepository cartItemRepository,
        CancellationToken cancellationToken)
    {
        if (!UserContext.TryGet(httpContext.User, out var userContext) ||
            (userContext.Role == ERequestClaimsRole.Customer && customerId != userContext.SystemUserId))
            return TypedResults.Forbid();

        var isDeleted = await cartItemRepository.Delete(customerId, productId, cancellationToken);
        if (!isDeleted) return TypedResults.NotFound();

        return TypedResults.Ok(new DeleteCartItemResponse
        {
            CustomerId = customerId,
            ProductId = productId
        });
    }

    public record DeleteCartItemResponse
    {
        public Guid CustomerId { get; init; }
        public Guid ProductId { get; init; }
        public string Message { get; init; } = "Cart item removed successfully.";
    }
}