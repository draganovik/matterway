namespace Matterway.Sales.Api.Domain.Entities;

using Domain;

public class OrderStatus
{
    public int Id { get; set; }
    public OrderId OrderId { get; set; }
    public Order? Order { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public EOrderStatusType Status { get; set; } = EOrderStatusType.Processing;
    public string? Note { get; set; }
}