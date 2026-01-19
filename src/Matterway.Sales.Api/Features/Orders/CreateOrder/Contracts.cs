using System.ComponentModel.DataAnnotations;
using Matterway.Sales.Api.Domain;

namespace Matterway.Sales.Api.Features.Orders.CreateOrder;

public sealed record CreateOrderRequest
{
    [Required]
    public Guid CustomerId { get; init; }

    public EOrderType? Type { get; init; }

    public DeliveryInfoRequest? DeliveryInfo { get; init; }
}

public sealed record DeliveryInfoRequest
{
    public string? Country { get; init; }

    [Required]
    public required string City { get; init; }

    [Required]
    public required string ZipCode { get; init; }

    [Required]
    public required string AddressLine1 { get; init; }

    public string? AddressLine2 { get; init; }

    public string? ContactPhone { get; init; }
}

public sealed record OrderResponse
{
    public Guid Id { get; init; }
    public Guid? CustomerId { get; init; }
    public EOrderType Type { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime PlacedAt { get; init; }
    public DeliveryInfoResponse? DeliveryInfo { get; init; }
    public IReadOnlyList<OrderItemResponse> Items { get; init; } = [];
    public IReadOnlyList<OrderStatusResponse> StatusHistory { get; init; } = [];
    public IReadOnlyList<PaymentSnapshotResponse> Payments { get; init; } = [];
}

public sealed record DeliveryInfoResponse
{
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? ZipCode { get; init; }
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? ContactPhone { get; init; }
}

public sealed record OrderItemResponse
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string? ProductTitle { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
}

public sealed record OrderStatusResponse
{
    public EOrderStatusType Status { get; init; }
    public DateTime ChangedAt { get; init; }
    public string? Note { get; init; }
}

public sealed record PaymentSnapshotResponse
{
    public Guid Id { get; init; }
    public string? Provider { get; init; }
    public string? ReferenceId { get; init; }
    public decimal Amount { get; init; }
    public EPaymentStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}