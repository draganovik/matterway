using SharedProject.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.API.Entities;

public class OrderHistory
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public Guid OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }
    [Required]
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public string? Description { get; set; }
    [Required]
    public DateTime CreatedDate { get; set; }
}
