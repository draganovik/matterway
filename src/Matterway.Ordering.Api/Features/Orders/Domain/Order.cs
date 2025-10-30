using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Matterway.Ordering.Api.Features.Addresses.Domain;
using Matterway.Ordering.Api.Features.OrderHistories.Domain;
using Matterway.Ordering.Api.Features.OrderItems.Domain;

namespace Matterway.Ordering.Api.Features.Orders.Domain;

public class Order
{
    [Key]
    public Guid Id { get; set; }

    public Guid? CustomerId { get; set; } = null;

    [Required]
    public Guid DeliveryAddressId { get; set; }

    [ForeignKey(nameof(DeliveryAddressId))]
    public Address? Address { get; set; }

    [Required]
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [Required]
    public ICollection<OrderHistory> OrderHistory { get; set; } = new List<OrderHistory>();

    [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}$",
        ErrorMessage = "Invalid ReferenceNumber. ReferenceNumber format must be: 0000-0000-0000")]
    public string ReferenceNumber { get; set; } = GenerateReferenceNumber();

    public static string GenerateReferenceNumber()
    {
        var random = new Random();
        var referenceNumber = string.Empty;
        for (var i = 0; i < 3; i++)
        {
            referenceNumber += random.Next(1000, 9999).ToString();
            if (i < 2) referenceNumber += "-";
        }

        return referenceNumber;
    }
}