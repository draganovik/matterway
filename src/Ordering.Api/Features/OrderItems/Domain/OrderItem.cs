using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Ordering.Api.Features.Orders.Domain;

namespace Ordering.Api.Features.OrderItems.Domain;

[PrimaryKey(nameof(OrderId), nameof(ProductId))]
public class OrderItem
{
    [Required]
    public Guid OrderId { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public string? ProductName { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public double UnitPrice { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}