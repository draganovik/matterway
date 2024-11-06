using System.Text.Json.Serialization;
using Shared.Enums;

namespace Ordering.API.Models.OrderHistoryModels;

public class OrderHistoryOrderResponseModel
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
}