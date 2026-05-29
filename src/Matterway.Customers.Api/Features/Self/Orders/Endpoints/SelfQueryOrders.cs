using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Self.Orders.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerOrderEntity;

namespace Matterway.Customers.Api.Features.Self.Orders.Endpoints;

public class SelfQueryOrders : IEndpoint
{
    private const string RouteName = nameof(SelfQueryOrders);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Self, "orders", Handler)
            .WithName(RouteName).WithSummary("[self] Query own orders.")
            .WithTags(nameof(CustomerOrder))
            .Produces<PaginationResponse<SelfBaseOrderResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<PaginationResponse<SelfBaseOrderResponse>>, NoContent, ForbidHttpResult>>
        Handler(
            [AsParameters]
            PaginationRequestParameters pagingQuery,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomerOrderRepository customerOrderRepository)
    {
        var customerId = RequestIdentity.GetIdentifier(httpContext.User);
        if (customerId is null) return TypedResults.Forbid();

        var total = await customerOrderRepository.CountForCustomer(customerId.Value);
        var entities = await customerOrderRepository.QueryForCustomer(customerId.Value, pagingQuery.Page,
            pagingQuery.PageSize);

        var baseUri = linkGenerator.GetUriByName(httpContext, RouteName, null);
        var response = entities.Select(ToResponse).ToList();

        var paginationResponse = PaginationResponse<SelfBaseOrderResponse>.Create(
            response,
            total,
            pagingQuery.Page,
            pagingQuery.PageSize,
            baseUri);

        if (entities.Count == 0) return TypedResults.NoContent();

        return TypedResults.Ok(paginationResponse);
    }

    private static SelfBaseOrderResponse ToResponse(CustomerOrder order)
    {
        var items = order.Items.Select(item => new SelfBaseOrderResponse.Item
        {
            ArticleCode = ArticleCode.Parse(item.ArticleCode, null),
            ArticleName = item.ArticleName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity
        }).ToList();

        var totalAmount = items.Sum(item => (item.UnitPrice ?? 0m) * item.Quantity);

        return new SelfBaseOrderResponse
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            PlacedAt = order.PlacedAt,
            TotalAmount = Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero),
            Items = items
        };
    }
}
