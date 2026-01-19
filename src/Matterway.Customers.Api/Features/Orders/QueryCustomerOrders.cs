using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerOrderEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Customers.Api.Features.Orders;

public class QueryCustomerOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Customers/Orders", Handler)
            .WithName("QueryCustomerOrders").WithSummary("Query customer orders.")
            .WithTags(nameof(CustomerOrder))
            .Produces<PaginationResponse<CustomerOrderResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireAuthorization()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestRole.Customer)))
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
        if (!RequestIdentity.TryGet(httpContext.User, out var userContext)) return TypedResults.Forbid();

        var total = await customerOrderRepository.CountForCustomer(userContext.SystemUserId);
        var entities = await customerOrderRepository.QueryForCustomer(userContext.SystemUserId, pagingQuery.Page,
            pagingQuery.PageSize);

        var baseUri = linkGenerator.GetUriByName(httpContext, "QueryCustomerOrders", null);
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
        public Guid ProductId { get; init; }

        [Required]
        public string? ProductName { get; init; }

        [Required]
        public decimal? UnitPrice { get; init; }

        [Required]
        public int Quantity { get; init; }
    }

    private static CustomerOrderResponse MapToResponse(CustomerOrder order)
    {
        var items = order.Items.Select(item => new CustomerArticleResponse
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
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