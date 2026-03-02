namespace Matterway.Sales.Api.Domain.Entities;

using Domain;

public class Order
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid? CustomerId { get; set; }
    public EOrderType Type { get; set; } = EOrderType.Ecommerce;
    public DateTime PlacedAt { get; init; } = DateTime.UtcNow;

    public OrderDeliveryInfo? DeliveryInfo { get; set; }
    public List<OrderItem> Items { get; } = [];
    public List<OrderStatus> StatusHistory { get; } = [];
    public List<Payment> Payments { get; } = [];
}