using Matterway.Common.Enums;
using System.Text.Json.Serialization;

namespace Matterway.Ordering.Api.Features.OrderHistories.Contracts;

public class OrderHistoryBaseResponse
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
}