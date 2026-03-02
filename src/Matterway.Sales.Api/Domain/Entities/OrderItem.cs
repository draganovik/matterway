namespace Matterway.Sales.Api.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid ArticleId { get; set; }
    public required string ArticleTitle { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}