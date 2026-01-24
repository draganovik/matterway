using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerOrderEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.Self.Orders;

public class SelfQueryOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("self/orders", Handler)
            .WithName("SelfQueryOrders").WithSummary("Query own orders.")
            .WithTags(nameof(CustomerOrder))
            .Produces<PaginationResponse<CustomerOrderResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<CustomerOrderResponse>>, NoContent, ForbidHttpResult>>
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

        var baseUri = linkGenerator.GetUriByName(httpContext, "SelfQueryOrders", null);
        var response = entities.Select(MapToResponse).ToList();

        var paginationResponse = PaginationResponse<CustomerOrderResponse>.Create(
            response,
            total,
            pagingQuery.Page,
            pagingQuery.PageSize,
            baseUri);

        if (entities.Count == 0) return TypedResults.NoContent();

        return TypedResults.Ok(paginationResponse);
    }

    public record CustomerOrderResponse
    {
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public DateTime PlacedAt { get; init; }
        public decimal TotalAmount { get; init; }
        public IReadOnlyList<CustomerArticleResponse> Items { get; init; } = [];
    }

    public record CustomerArticleResponse
    {
        [Required]
        public Guid ArticleId { get; init; }

        [Required]
        public string? ArticleName { get; init; }

        [Required]
        public decimal? UnitPrice { get; init; }

        [Required]
        public int Quantity { get; init; }
    }

    private static CustomerOrderResponse MapToResponse(CustomerOrder order)
    {
        var items = order.Items.Select(item => new CustomerArticleResponse
        {
            ArticleId = item.ArticleId,
            ArticleName = item.ArticleName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity
        }).ToList();

        var totalAmount = items.Sum(item => (item.UnitPrice ?? 0m) * item.Quantity);

        return new CustomerOrderResponse
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            PlacedAt = order.PlacedAt,
            TotalAmount = Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero),
            Items = items
        };
    }
}