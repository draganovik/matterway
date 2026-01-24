using System.Net;
using Matterway.Sales.Api.Application.Brokers;

namespace Matterway.Sales.Api.Infrastructure.Brokers.Customers;

public interface ICustomersClient
{
    Task<BrokerResponse<CustomersOrderResponse>> CreateOrderAsync(Guid customerId, CustomersCreateOrderRequest request,
        string? authorizationHeader,
        CancellationToken cancellationToken);
}

public sealed record CustomersCreateOrderRequest
{
    public Guid OrderId { get; init; }
    public CustomersDeliveryInfoRequest? DeliveryInfo { get; init; }
}

public sealed record CustomersDeliveryInfoRequest
{
    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string AddressLine2 { get; init; } = string.Empty;
    public string? ContactPhone { get; init; }
}

public sealed record CustomersOrderResponse
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime PlacedAt { get; init; }
    public decimal TotalAmount { get; init; }
    public CustomersDeliveryInfoResponse? DeliveryInfo { get; init; }
    public IReadOnlyList<CustomersOrderItemResponse> Items { get; init; } = [];
}

public sealed record CustomersDeliveryInfoResponse
{
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? ZipCode { get; init; }
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? ContactPhone { get; init; }
}

public sealed record CustomersOrderItemResponse
{
    public Guid ArticleId { get; init; }
    public string? ArticleName { get; init; }
    public decimal? UnitPrice { get; init; }
    public int Quantity { get; init; }
}