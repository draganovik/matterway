using Matterway.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ordering.Api.Features.OrderHistories.Contracts;

public class OrderHistoryBaseRequest
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public string? Description { get; set; }
}