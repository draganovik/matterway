using System.ComponentModel.DataAnnotations;

namespace Ordering.Api.Features.OrderItems.Contracts;

public class OrderItemBaseResponse
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public double UnitPrice { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}