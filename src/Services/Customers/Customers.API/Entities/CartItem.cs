using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Customers.API.Entities;

public class CartItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
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
