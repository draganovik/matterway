using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Brokers.Catalog;
using Matterway.Customers.Api.Infrastructure.Persistence.CartItemEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Customers.Api.Features.CartItems;

public class UpsertCartItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("Customers/{id:guid}/CartItems/{productId:guid}", Handler)
            .WithName("UpsertCartItemById").WithSummary("Upsert CartItem.")
            .WithTags(nameof(CartItem))
            .Produces<CartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
            Results<Ok<CartItemResponse>, NotFound, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(Guid id,
            Guid productId,
            CartItemRequest request,
            HttpContext httpContext,
            ICartItemRepository cartItemRepository,
            ICatalogClient catalogClient,
            CancellationToken cancellationToken)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out ERequestClaimsRole userRole))
            return TypedResults.Forbid();

        if (userRole == ERequestClaimsRole.Customer && id != systemUserId) return TypedResults.Forbid();

        var product = await catalogClient.GetProductById(productId, cancellationToken);
        if (product is null) return TypedResults.NotFound();

        var entity = new CartItem
        {
            CustomerId = id,
            ProductId = productId,
            Quantity = request.Quantity,
            ProductName = product.Title,
            UnitPrice = product.Price
        };

        try
        {
            var stored = await cartItemRepository.Upsert(entity);
            return stored is not null
                ? TypedResults.Ok(MapToResponse(stored))
                : TypedResults.NotFound();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
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
        [Range(0.01, double.MaxValue)]
        public double UnitPrice { get; init; }
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