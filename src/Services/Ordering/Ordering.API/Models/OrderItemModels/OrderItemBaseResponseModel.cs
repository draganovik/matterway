namespace Ordering.API.Models.OrderItemModels;

public class OrderItemBaseResponseModel
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public double UnitPrice { get; set; }
    public int Units { get; set; }
}
