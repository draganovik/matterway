using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.API.Entities;

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
    public ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();

    // Validate reference number to be in format of paycheck referene number 
    [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}$")]
    public string ReferenceNumber { get; set; } = GenerateReferenceNumber();

    public static string GenerateReferenceNumber()
    {
        var random = new Random();
        var referenceNumber = string.Empty;
        for (int i = 0; i < 4; i++)
        {
            referenceNumber += random.Next(1000, 9999).ToString();
            if (i < 3)
            {
                referenceNumber += "-";
            }
        }
        return referenceNumber;
    }
}
