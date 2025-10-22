using System.Text.Json.Serialization;
using Common.Infrastructure.Enums;

namespace Ordering.Api.Features.OrderHistories.Contracts;

public class OrderHistoryOrderResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
}