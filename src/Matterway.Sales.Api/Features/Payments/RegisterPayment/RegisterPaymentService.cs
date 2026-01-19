using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;

namespace Matterway.Sales.Api.Features.Payments.RegisterPayment;

public sealed class RegisterPaymentService(
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository)
{
    public Task<bool> OrderExistsAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return orderRepository.Exists(orderId, cancellationToken);
    }

    public Payment BuildPayment(RegisterPaymentCommand command)
    {
        return new Payment
        {
            OrderId = command.OrderId,
            Provider = command.Provider,
            ReferenceId = command.ReferenceId,
            Amount = command.Amount,
            Status = command.Status,
            CreatedAt = command.CreatedAt ?? DateTime.UtcNow
        };
    }

    public Task<Payment?> PersistAsync(Payment payment, CancellationToken cancellationToken)
    {
        return paymentRepository.Create(payment, cancellationToken);
    }
}

public sealed record RegisterPaymentCommand(
    Guid OrderId,
    string Provider,
    string ReferenceId,
    decimal Amount,
    EPaymentStatus Status,
    DateTime? CreatedAt);