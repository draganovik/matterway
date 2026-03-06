using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

namespace Matterway.Sales.Api.Endpoints.Self.Orders;

public class SelfGetOrderById : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Self, "orders/{orderId:OrderId}", Handler)
            .WithName("SelfGetOrderById").WithSummary("[self] Get own Order by id.")
            .WithTags(nameof(Order))
            .Produces<OrderResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<OrderResponse>, NotFound, ForbidHttpResult>> Handler(
        OrderId orderId,
        HttpContext httpContext,
        IOrderRepository orderRepository,
        CancellationToken cancellationToken)
    {
        var customerId = RequestIdentity.GetIdentifier(httpContext.User);
        if (customerId is null) return TypedResults.Forbid();

        var entity = await orderRepository.GetById(orderId, cancellationToken);
        if (entity is null || entity.CustomerId != customerId) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(entity));
    }

    public record OrderResponse
    {
        public OrderId Id { get; init; }
        public Guid? CustomerId { get; init; }
        public EOrderType Type { get; init; }
        public decimal TotalAmount { get; init; }
        public DateTime PlacedAt { get; init; }
        public DeliveryInfoResponse? DeliveryInfo { get; init; }
        public IReadOnlyList<OrderItemResponse> Items { get; init; } = [];
        public IReadOnlyList<OrderStatusResponse> StatusHistory { get; init; } = [];
        public IReadOnlyList<PaymentSnapshotResponse> Payments { get; init; } = [];
    }

    public record DeliveryInfoResponse
    {
        public string? Country { get; init; }
        public string? City { get; init; }
        public string? ZipCode { get; init; }
        public string? AddressLine1 { get; init; }
        public string? AddressLine2 { get; init; }
        public string? ContactPhone { get; init; }
    }

    public record OrderItemResponse
    {
        public Guid Id { get; init; }
        public ArticleCode ArticleCode { get; init; }
        public string? ArticleTitle { get; init; }
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }

    public record OrderStatusResponse
    {
        public EOrderStatusType Status { get; init; }
        public DateTime ChangedAt { get; init; }
        public string? Note { get; init; }
    }

    public record PaymentSnapshotResponse
    {
        public Guid Id { get; init; }
        public string? Provider { get; init; }
        public decimal Amount { get; init; }
        public EPaymentStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    private static decimal CalculateTotal(IEnumerable<OrderItem> items)
    {
        var total = items.Sum(item => item.UnitPrice * item.Quantity);
        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    internal static OrderResponse MapToResponse(Order entity)
    {
        return new OrderResponse
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Type = entity.Type,
            TotalAmount = CalculateTotal(entity.Items),
            PlacedAt = entity.PlacedAt,
            DeliveryInfo = entity.DeliveryInfo is null
                ? null
                : new DeliveryInfoResponse
                {
                    Country = entity.DeliveryInfo.Country,
                    City = entity.DeliveryInfo.City,
                    ZipCode = entity.DeliveryInfo.ZipCode,
                    AddressLine1 = entity.DeliveryInfo.AddressLine1,
                    AddressLine2 = entity.DeliveryInfo.AddressLine2,
                    ContactPhone = entity.DeliveryInfo.ContactPhone
                },
            Items = entity.Items.Select(item => new OrderItemResponse
            {
                Id = item.Id,
                ArticleCode = ArticleCode.Parse(item.ArticleCode, null),
                ArticleTitle = item.ArticleTitle,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            }).ToList(),
            StatusHistory = entity.StatusHistory.Select(status => new OrderStatusResponse
            {
                Status = status.Status,
                ChangedAt = status.ChangedAt,
                Note = status.Note
            }).ToList(),
            Payments = entity.Payments.Select(payment => new PaymentSnapshotResponse
            {
                Id = payment.Id,
                Provider = payment.Provider,
                Amount = payment.Amount,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
            }).ToList()
        };
    }
}