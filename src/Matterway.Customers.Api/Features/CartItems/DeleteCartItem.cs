using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using Matterway.Customers.Api.Infrastructure.Persistence.EntityCartItem;

namespace Matterway.Customers.Api.Features.CartItems;

public class DeleteCartItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Customers/{id:guid}/CartItems/{productId:guid}", Handler)
            .WithName("DeleteCartItem").WithSummary("Delete CartItem.")
            .WithTags(nameof(CartItem))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Handler(
        Guid id,
        Guid productId,
        HttpContext httpContext,
        ICartItemRepository cartItemRepository)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out ERequestClaimsRole userRole))
            return TypedResults.Forbid();

        if (userRole != ERequestClaimsRole.Admin && id != systemUserId) return TypedResults.Forbid();

        var isDeleted = await cartItemRepository.Delete(id, productId);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}