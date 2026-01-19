using System.ComponentModel.DataAnnotations;
using Matterway.Sales.Api.Domain;

namespace Matterway.Sales.Api.Features.Payments.RegisterPayment;

public sealed record RegisterPaymentRequest
{
    [Required]
    public Guid OrderId { get; init; }

    [Required]
    public required string Provider { get; init; }

    [Required]
    [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}$",
        ErrorMessage = "Invalid ReferenceId. ReferenceId format must be: 0000-0000-0000-0000")]
    public required string ReferenceId { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; init; }

    public EPaymentStatus Status { get; init; } = EPaymentStatus.Reserved;

    public DateTime? CreatedAt { get; init; }
}

public sealed record PaymentResponse
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public string? Provider { get; init; }
    public string? ReferenceId { get; init; }
    public decimal Amount { get; init; }
    public EPaymentStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}