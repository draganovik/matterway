using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.CartItems;

public class DeleteCartItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Customers/{customerId:guid}/CartItems/{articleId:guid}", Handler)
            .WithName("DeleteCartItem").WithSummary("Delete CartItem.")
            .WithTags(nameof(CustomerArticle))
            .Produces<DeleteCartItemResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteCartItemResponse>, NotFound, ForbidHttpResult>> Handler(
        Guid customerId,
        Guid articleId,
        HttpContext httpContext,
        ICustomerArticleRepository cartItemRepository,
        CancellationToken cancellationToken)
    {
        if (!RequestIdentity.TryGet(httpContext.User, out var userContext) ||
            (userContext.Role == ERequestRole.Customer && customerId != userContext.SystemUserId))
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