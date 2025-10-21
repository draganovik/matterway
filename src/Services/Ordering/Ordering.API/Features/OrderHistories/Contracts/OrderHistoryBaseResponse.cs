using System.Text.Json.Serialization;
using Shared.Enums;

namespace Ordering.API.Features.OrderHistories.Contracts;

public class OrderHistoryBaseResponse
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
}