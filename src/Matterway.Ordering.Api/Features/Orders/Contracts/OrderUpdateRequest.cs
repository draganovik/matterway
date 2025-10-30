using System.ComponentModel.DataAnnotations;

namespace Matterway.Ordering.Api.Features.Orders.Contracts;

public class OrderUpdateRequest
{
    public Guid? CustomerId { get; set; } = null;

    [Required]
    public Guid DeliveryAddressId { get; set; }

    [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}$",
        ErrorMessage = "Invalid ReferenceNumber. ReferenceNumber format must be: 0000-0000-0000")]
    public string? ReferenceNumber { get; set; }
}