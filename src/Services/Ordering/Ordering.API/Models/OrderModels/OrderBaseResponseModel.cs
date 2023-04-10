using Ordering.API.Entities;
using Ordering.API.Models.AddressModels;
using Ordering.API.Models.OrderHistoryModels;
using Ordering.API.Models.OrderItemModels;
using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Models.OrderModels;

public class OrderBaseResponseModel
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public AddressOrderResponseModel? Address { get; set; }
    public ICollection<OrderItemOrderResponseModel> OrderItems { get; set; } = new List<OrderItemOrderResponseModel>();
    public ICollection<OrderHistoryOrderResponseModel> OrderHistory { get; set; } = new List<OrderHistoryOrderResponseModel>();
    [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}$", ErrorMessage = "Invalid ReferenceNumber. ReferenceNumber format must be: 0000-0000-0000-0000")]
    public string? ReferenceNumber { get; set; }
}
