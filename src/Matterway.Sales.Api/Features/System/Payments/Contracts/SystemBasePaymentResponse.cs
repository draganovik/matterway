using Matterway.Sales.Api.Domain;

namespace Matterway.Sales.Api.Features.System.Payments.Contracts;

public record SystemBasePaymentResponse
{
    public Guid Id { get; init; }
    public OrderId OrderId { get; init; }
    public string? Provider { get; init; }
    public decimal Amount { get; init; }
    public EPaymentStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}
