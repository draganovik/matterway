using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Models.OrderModels;

public class OrderUpdateRequestModel
{
    public Guid? CustomerId { get; set; } = null;
    [Required]
    public Guid DeliveryAddressId { get; set; }
    [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}$", ErrorMessage = "Invalid ReferenceNumber. ReferenceNumber format must be: 0000-0000-0000-0000")]
    public string? ReferenceNumber { get; set; }
}
