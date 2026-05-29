using Matterway.Sales.Api.Domain;

namespace Matterway.Sales.Api.Features.System.Orders.Contracts;

public record SystemCreateOrderResponse
{
    public OrderId Id { get; init; }
    public Guid? CustomerId { get; init; }
    public EOrderType Type { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime PlacedAt { get; init; }
}
