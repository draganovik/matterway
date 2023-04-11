using SharedProject.Enums;
using System.ComponentModel.DataAnnotations;

namespace Payments.API.Entities;

public class Payment
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}$", ErrorMessage = "Invalid ReferenceNumber. ReferenceNumber format must be: 0000-0000-0000-0000")]
    public string? ReferenceNumber { get; set; }
    [Required]
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public double PaymentAmount { get; set; }
    [Required]
    [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}$", ErrorMessage = "Invalid CardNumber. CardNumber format must be: 0000-0000-0000-0000")]
    public string? CardNumber { get; set; }
    [Required]
    public string? CardHolder { get; set; }
    [Required]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{4}|[0-9]{2})$", ErrorMessage = "Invalid ExpirationDate. ExpirationDate format must be: MM/YY")]
    public string? ExpirationDate { get; set; }
    [Required]
    [Range(0, 999, ErrorMessage = "SecurityCode must be between 0 and 999")]
    public string? SecurityCode { get; set; }
    public PaymentState PaymentState { get; set; } = PaymentState.Pending;
}
