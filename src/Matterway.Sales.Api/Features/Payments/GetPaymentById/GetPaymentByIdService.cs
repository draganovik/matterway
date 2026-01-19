using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;

namespace Matterway.Sales.Api.Features.Payments.GetPaymentById;

public sealed class GetPaymentByIdService(IPaymentRepository paymentRepository)
{
    public Task<Payment?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        return paymentRepository.GetById(paymentId, cancellationToken);
    }
}