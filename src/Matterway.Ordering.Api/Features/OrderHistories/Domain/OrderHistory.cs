using Matterway.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Matterway.Ordering.Api.Features.Orders.Domain;

namespace Matterway.Ordering.Api.Features.OrderHistories.Domain;

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
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}