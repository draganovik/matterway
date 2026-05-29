using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Self.CartItems.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

namespace Matterway.Customers.Api.Features.Self.CartItems.Endpoints;

public class SelfQueryCartItems : IEndpoint
{
    private const string RouteName = nameof(SelfQueryCartItems);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Self, "cart/items", Handler)
            .WithName(RouteName).WithSummary("[self] Query own CartItems.")
            .WithTags(nameof(CustomerArticle))
            .Produces<PaginationResponse<SelfBaseCartItemResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<PaginationResponse<SelfBaseCartItemResponse>>, NoContent, ForbidHttpResult>>
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

        var baseUri = linkGenerator.GetUriByName(httpContext, RouteName, null);
        var response = entities.Select(ToResponse).ToList();

        var paginationResponse = PaginationResponse<SelfBaseCartItemResponse>.Create(
            response,
            total,
            pagingQuery.Page,
            pagingQuery.PageSize,
            baseUri);

        if (entities.Count == 0) return TypedResults.NoContent();

        return TypedResults.Ok(paginationResponse);
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