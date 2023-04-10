using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.API.Entities;

public class OrderItem
{
    [Key]
    public Guid Id { get; set; }
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
    [Range(1, int.MaxValue, ErrorMessage = "Units must be greater than 0")]
    public int Units { get; set; }
}
