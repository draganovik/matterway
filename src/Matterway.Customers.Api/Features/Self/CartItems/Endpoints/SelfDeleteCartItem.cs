using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Self.CartItems.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

namespace Matterway.Customers.Api.Features.Self.CartItems.Endpoints;

public class SelfDeleteCartItem : IEndpoint
{
    private const string RouteName = nameof(SelfDeleteCartItem);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Self, "customers/{customerId:guid}/cart-items/{article:ArticleCode}", Handler)
            .WithName(RouteName).WithSummary("[self] Delete own CartItem.")
            .WithTags(nameof(CustomerArticle))
            .Produces<SelfDeleteCartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.IsCustomer(context.User) ||
                RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<SelfDeleteCartItemResponse>, NotFound, ForbidHttpResult>> Handler(
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

        return TypedResults.Ok(new SelfDeleteCartItemResponse
        {
            CustomerId = customerId,
            ArticleCode = article
        });
    }
}