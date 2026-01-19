using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Providers.Brokers.Catalog;
using Matterway.Customers.Api.Providers.Persistence.CustomerArticleEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Customers.Api.Features.CartItems;

public class UpsertCartItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("Customers/{customerId:guid}/CartItems/{productId:guid}", Handler)
            .WithName("UpsertCartItemById").WithSummary("Upsert CartItem.")
            .WithTags(nameof(CustomerArticle))
            .Produces<CartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
            Results<Ok<CartItemResponse>, NotFound, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(Guid customerId,
            Guid productId,
            CartItemRequest request,
            HttpContext httpContext,
            ICustomerArticleRepository cartItemRepository,
            ICatalogClient catalogClient,
            CancellationToken cancellationToken)
    {
        if (!RequestIdentity.TryGet(httpContext.User, out var userContext) ||
            (userContext.Role == ERequestRole.Customer && customerId != userContext.SystemUserId))
            return TypedResults.Forbid();

        var product = await catalogClient.GetProductById(productId, cancellationToken);
        if (product is null) return TypedResults.NotFound();

        var resolvedPrice = product.Price ?? product.BasePrice;
        if (resolvedPrice is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Product price is unavailable."
            });

        var entity = new CustomerArticle
        {
            CustomerId = customerId,
            ProductId = productId,
            Quantity = request.Quantity,
            ProductName = product.Title,
            UnitPrice = resolvedPrice
        };

        try
        {
            var stored = await cartItemRepository.UpsertCartItem(entity, cancellationToken);
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
        [Range(typeof(decimal), "0.01", "2147483647")]
        public decimal? UnitPrice { get; init; }
    }

    public record CartItemRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }
    }

    private static CartItemResponse MapToResponse(CustomerArticle entity)
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