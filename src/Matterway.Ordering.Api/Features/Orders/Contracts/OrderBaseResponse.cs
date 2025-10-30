using System.ComponentModel.DataAnnotations;
using Matterway.Ordering.Api.Features.Addresses.Contracts;
using Matterway.Ordering.Api.Features.OrderHistories.Contracts;
using Matterway.Ordering.Api.Features.OrderItems.Contracts;

namespace Matterway.Ordering.Api.Features.Orders.Contracts;

public class OrderBaseResponse
{
    public Guid Id { get; set; }

    public Guid? CustomerId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Total must be greater than 0")]
    public double Total { get; set; }

    public AddressOrderResponse? Address { get; set; }
    public ICollection<OrderItemOrderResponse> OrderItems { get; set; } = new List<OrderItemOrderResponse>();

    public ICollection<OrderHistoryOrderResponse> OrderHistory { get; set; } =
        new List<OrderHistoryOrderResponse>();

    [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}$",
        ErrorMessage = "Invalid ReferenceNumber. ReferenceNumber format must be: 0000-0000-0000")]
    public string? ReferenceNumber { get; set; }
}