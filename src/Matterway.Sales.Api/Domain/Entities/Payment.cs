namespace Matterway.Sales.Api.Domain.Entities;

using Domain;

public class Payment
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public OrderId OrderId { get; set; }
    public Order? Order { get; set; }

    public required string Provider { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public EPaymentStatus Status { get; set; } = EPaymentStatus.Reserved;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}