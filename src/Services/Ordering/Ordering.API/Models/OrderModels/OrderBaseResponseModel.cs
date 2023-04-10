using Ordering.API.Entities;
using Ordering.API.Models.OrderHistoryModels;

namespace Ordering.API.Models.OrderModels;

public class OrderBaseResponseModel
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public Address? Address { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<OrderHistoryBaseResponseModel> OrderHistory { get; set; } = new List<OrderHistoryBaseResponseModel>();
    public string? ReferenceNumber { get; set; }
}
