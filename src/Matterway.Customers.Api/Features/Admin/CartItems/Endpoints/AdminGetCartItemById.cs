using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Admin.CartItems.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

namespace Matterway.Customers.Api.Features.Admin.CartItems.Endpoints;

public class AdminGetCartItemById : IEndpoint
{
    private const string RouteName = nameof(AdminGetCartItemById);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "customers/{customerId:guid}/cart-items/{article:ArticleCode}", Handler)
            .WithName(RouteName).WithSummary("[admin] Get CartItem by id")
            .WithTags(nameof(CustomerArticle))
            .Produces<AdminGetCartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminGetCartItemResponse>, NotFound>> Handler(
        Guid customerId,
        ArticleCode article,
        ICustomerArticleRepository cartItemRepository,
        CancellationToken cancellationToken)
    {
        var entity = await cartItemRepository.GetCartItem(customerId, article, cancellationToken);
        if (entity == null) return TypedResults.NotFound();

        return TypedResults.Ok(ToResponse(entity));
    }

    private static AdminGetCartItemResponse ToResponse(CustomerArticle entity)
    {
        return new AdminGetCartItemResponse
        {
            CustomerId = entity.CustomerId,
            ArticleCode = ArticleCode.Parse(entity.ArticleCode, null),
            ArticleName = entity.ArticleName,
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice
        };
    }
}
