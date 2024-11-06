using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Customers.API.Entities;

[PrimaryKey(nameof(CustomerId), nameof(ProductId))]
public class CartItem
{
    [Required]
    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer? Customer { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public double UnitPrice { get; set; }

    [Required]
    public string? ProductName { get; set; }
}