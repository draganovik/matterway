using System.ComponentModel.DataAnnotations;
using System.Net;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Brokers.Catalog;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

namespace Matterway.Customers.Api.Endpoints.Self.CartItems;

public class SelfUpsertCartItem : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPut(EndpointKind.Self, "customers/{customerId:guid}/cart-items/{article:ArticleCode}", Handler)
            .WithName("SelfUpsertCartItem").WithSummary("[self] Upsert own CartItem.")
            .WithTags(nameof(CustomerArticle))
            .Produces<CartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.IsCustomer(context.User) ||
                RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<
            Results<Ok<CartItemResponse>, NotFound, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(Guid customerId,
            ArticleCode article,
            CartItemRequest request,
            HttpContext httpContext,
            ICustomerArticleRepository cartItemRepository,
            ICatalogClient catalogClient,
            CancellationToken cancellationToken)
    {
        if (!RequestIdentity.CanManageOwnedResource(httpContext.User, customerId))
            return TypedResults.Forbid();

        var articleResponse = await catalogClient.GetArticleByCode(article, cancellationToken);
        if (!articleResponse.IsSuccess || articleResponse.Data is null)
            return articleResponse.StatusCode == HttpStatusCode.NotFound
                ? TypedResults.NotFound()
                : TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = articleResponse.ErrorMessage ?? "Unable to retrieve article."
                });

        var catalogArticle = articleResponse.Data;

        var resolvedPrice = catalogArticle.Price ?? catalogArticle.BasePrice;
        if (resolvedPrice is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Article price is unavailable."
            });

        var entity = new CustomerArticle
        {
            CustomerId = customerId,
            Quantity = request.Quantity,
            ArticleName = catalogArticle.Title,
            ArticleCode = catalogArticle.Code.ToString(),
            UnitPrice = resolvedPrice
        };

        var stored = (await cartItemRepository.UpsertCartItem(entity, cancellationToken))!;
        return TypedResults.Ok(MapToResponse(stored));
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
            ArticleName = entity.ArticleName,
            ArticleCode = ArticleCode.Parse(entity.ArticleCode, null),
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice
        };
    }
}