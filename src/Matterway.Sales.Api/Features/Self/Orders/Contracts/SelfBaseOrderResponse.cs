using Matterway.Sales.Api.Domain;

namespace Matterway.Sales.Api.Features.Self.Orders.Contracts;

public record SelfBaseOrderResponse
{
    public OrderId Id { get; init; }
    public Guid? CustomerId { get; init; }
    public EOrderType Type { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime PlacedAt { get; init; }
    public DeliveryInfoResponse? DeliveryInfo { get; init; }
    public IReadOnlyList<ItemResponse> Items { get; init; } = [];
    public IReadOnlyList<StatusResponse> StatusHistory { get; init; } = [];
    public IReadOnlyList<PaymentSnapshotResponse> Payments { get; init; } = [];

    public record DeliveryInfoResponse
    {
        public string? Country { get; init; }
        public string? City { get; init; }
        public string? ZipCode { get; init; }
        public string? AddressLine1 { get; init; }
        public string? AddressLine2 { get; init; }
        public string? ContactPhone { get; init; }
    }

    public record ItemResponse
    {
        public Guid Id { get; init; }
        public ArticleCode ArticleCode { get; init; }
        public string? ArticleTitle { get; init; }
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }

    public record StatusResponse
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
}
