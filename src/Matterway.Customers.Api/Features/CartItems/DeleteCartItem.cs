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
        app.MapDelete("Customers/{id:guid}/CartItems/{productId:guid}", Handler)
            .WithName("DeleteCartItem").WithSummary("Delete CartItem.")
            .WithTags(nameof(CartItem))
            .Produces<DeleteCartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteCartItemResponse>, NotFound, ForbidHttpResult>> Handler(
        Guid id,
        Guid productId,
        HttpContext httpContext,
        ICartItemRepository cartItemRepository,
        CancellationToken cancellationToken)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out ERequestClaimsRole userRole))
            return TypedResults.Forbid();

        if (userRole == ERequestClaimsRole.Customer && id != systemUserId) return TypedResults.Forbid();

        var isDeleted = await cartItemRepository.Delete(id, productId, cancellationToken);
        return isDeleted
            ? TypedResults.Ok(new DeleteCartItemResponse
            {
                CustomerId = id,
                ProductId = productId
            })
            : TypedResults.NotFound();
    }

    public record DeleteCartItemResponse
    {
        public Guid CustomerId { get; init; }
        public Guid ProductId { get; init; }
        public string Message { get; init; } = "Cart item removed successfully.";
    }
}