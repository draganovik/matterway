namespace Matterway.Customers.Api.Features.System.Orders.Contracts;

public record SystemCreateOrderResponse
{
    public OrderId OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime PlacedAt { get; init; }
    public decimal TotalAmount { get; init; }
    public DeliveryInfoResponse? DeliveryInfo { get; init; }
    public IReadOnlyList<Item> Items { get; init; } = [];

    public record DeliveryInfoResponse
    {
        public string? Country { get; init; }
        public string? City { get; init; }
        public string? ZipCode { get; init; }
        public string? AddressLine1 { get; init; }
        public string? AddressLine2 { get; init; }
        public string? ContactPhone { get; init; }
    }

    public record Item
    {
        public ArticleCode ArticleCode { get; init; }
        public string? ArticleName { get; init; }
        public decimal? UnitPrice { get; init; }
        public int Quantity { get; init; }
    }
}