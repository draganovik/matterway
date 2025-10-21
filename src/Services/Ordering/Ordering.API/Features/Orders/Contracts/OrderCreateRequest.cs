using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Features.Orders.Contracts;

public class OrderCreateRequest
{
    public Guid? CustomerId { get; set; } = null;

    [Required]
    public Guid DeliveryAddressId { get; set; }
}