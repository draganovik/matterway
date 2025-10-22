using Asp.Versioning;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Enums;
using Customers.Api.Features.CartItems.Data;
using Customers.Api.Features.CartItems.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace Customers.Api.Features.CartItems.Endpoints;

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

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
            return TypedResults.Forbid();

        if (userRole != SystemUserRole.Admin && id != systemUserId)
        {
            return TypedResults.Forbid();
        }

        var isDeleted = await cartItemRepository.Delete(id, productId);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}