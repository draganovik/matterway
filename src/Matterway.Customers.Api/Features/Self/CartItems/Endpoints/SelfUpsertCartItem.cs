using System.Net;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Self.CartItems.Contracts;
using Matterway.Customers.Api.Infrastructure.Brokers.Catalog;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

namespace Matterway.Customers.Api.Features.Self.CartItems.Endpoints;

public class SelfUpsertCartItem : IEndpoint
{
    private const string RouteName = nameof(SelfUpsertCartItem);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPut(EndpointKind.Self, "cart/items/{article:ArticleCode}", Handler)
            .WithName(RouteName).WithSummary("[self] Upsert own CartItem.")
            .WithTags(nameof(CustomerArticle))
            .Produces<SelfBaseCartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<
            Results<Ok<SelfBaseCartItemResponse>, NotFound, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(ArticleCode article,
            SelfUpsertCartItemRequest request,
            HttpContext httpContext,
            ICustomerArticleRepository cartItemRepository,
            ICatalogClient catalogClient,
            CancellationToken cancellationToken)
    {
        var customerId = RequestIdentity.GetIdentifier(httpContext.User);
        if (customerId is null) return TypedResults.Forbid();

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
            CustomerId = customerId.Value,
            Quantity = request.Quantity,
            ArticleName = catalogArticle.Title,
            ArticleCode = catalogArticle.Code.ToString(),
            UnitPrice = resolvedPrice
        };

        var stored = (await cartItemRepository.UpsertCartItem(entity, cancellationToken))!;
        return TypedResults.Ok(ToResponse(stored));
    }

    private static SelfBaseCartItemResponse ToResponse(CustomerArticle entity)
    {
        return new SelfBaseCartItemResponse
        {
            CustomerId = entity.CustomerId,
            ArticleName = entity.ArticleName,
            ArticleCode = ArticleCode.Parse(entity.ArticleCode, null),
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice
        };
    }
}
