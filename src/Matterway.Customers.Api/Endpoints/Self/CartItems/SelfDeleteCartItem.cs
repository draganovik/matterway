using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

namespace Matterway.Customers.Api.Endpoints.Self.CartItems;

public class SelfDeleteCartItem : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Self, "customers/{customerId:guid}/cart-items/{article:ArticleCode}", Handler)
            .WithName("SelfDeleteCartItem").WithSummary("[self] Delete own CartItem.")
            .WithTags(nameof(CustomerArticle))
            .Produces<DeleteCartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.IsCustomer(context.User) ||
                RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<DeleteCartItemResponse>, NotFound, ForbidHttpResult>> Handler(
        Guid customerId,
        ArticleCode article,
        HttpContext httpContext,
        ICustomerArticleRepository cartItemRepository,
        CancellationToken cancellationToken)
    {
        if (!RequestIdentity.CanManageOwnedResource(httpContext.User, customerId))
            return TypedResults.Forbid();

        var isDeleted = await cartItemRepository.DeleteCartItem(customerId, article, cancellationToken);
        if (!isDeleted) return TypedResults.NotFound();

        return TypedResults.Ok(new DeleteCartItemResponse
        {
            CustomerId = customerId,
            ArticleCode = article
        });
    }

    public record DeleteCartItemResponse
    {
        public Guid CustomerId { get; init; }
        public ArticleCode ArticleCode { get; init; }
        public string Message { get; init; } = "Cart item removed successfully.";
    }
}