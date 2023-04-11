using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Models.OrderModels;

public class OrderCreateRequestModel
{
    public Guid? CustomerId { get; set; } = null;
    [Required]
    public Guid DeliveryAddressId { get; set; }
}
