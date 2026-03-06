using System.ComponentModel.DataAnnotations;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

namespace Matterway.Customers.Api.Endpoints.Self.CartItems;

public class SelfQueryCartItems : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Self, "cart/items", Handler)
            .WithName("SelfQueryCartItems").WithSummary("[self] Query own CartItems.")
            .WithTags(nameof(CustomerArticle))
            .Produces<PaginationResponse<CartItemResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<PaginationResponse<CartItemResponse>>, NoContent, ForbidHttpResult>>
        Handler(
            [AsParameters]
            PaginationRequestParameters pagingQuery,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomerArticleRepository cartItemRepository)
    {
        var customerId = RequestIdentity.GetIdentifier(httpContext.User);
        if (customerId is null) return TypedResults.Forbid();

        var total = await cartItemRepository.CountCart(customerId.Value);
        var entities = await cartItemRepository.QueryCart(customerId.Value, pagingQuery.Page,
            pagingQuery.PageSize);

        var baseUri = linkGenerator.GetUriByName(httpContext, "QueryCartItems", null);
        var response = entities.Select(MapToResponse).ToList();

        var paginationResponse = PaginationResponse<CartItemResponse>.Create(
            response,
            total,
            pagingQuery.Page,
            pagingQuery.PageSize,
            baseUri);

        if (entities.Count == 0) return TypedResults.NoContent();

        return TypedResults.Ok(paginationResponse);
    }

    public record CartItemResponse
    {
        [Required]
        public Guid CustomerId { get; init; }

        [Required]
        public string? ArticleName { get; init; }

        [Required]
        public ArticleCode ArticleCode { get; init; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }

        [Required]
        [Range(0.01, int.MaxValue)]
        public decimal? UnitPrice { get; init; }
    }

    private static CartItemResponse MapToResponse(CustomerArticle entity)
    {
        return new CartItemResponse
        {
            CustomerId = entity.CustomerId,
            ArticleName = entity.ArticleName,
            ArticleCode = ArticleCode.Parse(entity.ArticleCode, null),
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice
        };
    }
}