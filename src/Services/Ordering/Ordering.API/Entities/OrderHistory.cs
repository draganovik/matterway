using Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public string? Description { get; set; }
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
