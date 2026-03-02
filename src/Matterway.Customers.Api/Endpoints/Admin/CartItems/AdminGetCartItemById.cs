using System.ComponentModel.DataAnnotations;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

namespace Matterway.Customers.Api.Endpoints.Admin.CartItems;

public class AdminGetCartItemById : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "customers/{customerId:guid}/cart-items/{articleId:guid}", Handler)
            .WithName("AdminGetCartItemById").WithSummary("[admin] Get CartItem by id")
            .WithTags(nameof(CustomerArticle))
            .Produces<CartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<CartItemResponse>, NotFound>> Handler(
        Guid customerId,
        Guid articleId,
        ICustomerArticleRepository cartItemRepository,
        CancellationToken cancellationToken)
    {
        var entity = await cartItemRepository.GetCartItem(customerId, articleId, cancellationToken);
        if (entity == null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(entity));
    }

    public record CartItemResponse
    {
        [Required]
        public Guid CustomerId { get; init; }

        [Required]
        public string? ArticleName { get; init; }

        [Required]
        public Guid ArticleId { get; init; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }

        [Required]
        [Range(typeof(decimal), "0.01", "2147483647")]
        public decimal? UnitPrice { get; init; }
    }

    private static CartItemResponse MapToResponse(CustomerArticle entity)
    {
        return new CartItemResponse
        {
            CustomerId = entity.CustomerId,
            ArticleId = entity.ArticleId,
            ArticleName = entity.ArticleName,
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice
        };
    }
}