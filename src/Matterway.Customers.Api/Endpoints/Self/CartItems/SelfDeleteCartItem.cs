using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

namespace Matterway.Customers.Api.Endpoints.Self.CartItems;

public class SelfDeleteCartItem : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Self, "customers/{customerId:guid}/cart-items/{articleId:guid}", Handler)
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
        Guid articleId,
        HttpContext httpContext,
        ICustomerArticleRepository cartItemRepository,
        CancellationToken cancellationToken)
    {
        if (!RequestIdentity.CanManageOwnedResource(httpContext.User, customerId))
            return TypedResults.Forbid();

        var isDeleted = await cartItemRepository.DeleteCartItem(customerId, articleId, cancellationToken);
        if (!isDeleted) return TypedResults.NotFound();

        return TypedResults.Ok(new DeleteCartItemResponse
        {
            CustomerId = customerId,
            ArticleId = articleId
        });
    }

    public record DeleteCartItemResponse
    {
        public Guid CustomerId { get; init; }
        public Guid ArticleId { get; init; }
        public string Message { get; init; } = "Cart item removed successfully.";
    }
}