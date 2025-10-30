using Matterway.Common.Enums;
using System.Text.Json.Serialization;

namespace Matterway.Ordering.Api.Features.OrderHistories.Contracts;

public class OrderHistoryOrderResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
}