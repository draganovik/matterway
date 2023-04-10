using System.ComponentModel.DataAnnotations;
using SharedProject.Enums;
using System.Text.Json.Serialization;

namespace Ordering.API.Models.OrderHistoryModels
{
    public class OrderHistoryBaseRequestModel
    {
        [Required]
        public Guid OrderId { get; set; }
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public string? Description { get; set; }
    }
}
