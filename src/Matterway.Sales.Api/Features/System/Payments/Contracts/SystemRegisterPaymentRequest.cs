using System.ComponentModel.DataAnnotations;
using Matterway.Sales.Api.Domain;

namespace Matterway.Sales.Api.Features.System.Payments.Contracts;

public record SystemRegisterPaymentRequest
{
    [Required]
    [OrderId]
    public string? OrderId { get; init; }

    [Required]
    public string? Provider { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal? Amount { get; init; }

    public EPaymentStatus Status { get; init; } = EPaymentStatus.Reserved;

    public DateTime? CreatedAt { get; init; }
}