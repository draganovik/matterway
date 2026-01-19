using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;

namespace Matterway.Sales.Api.Features.Payments.QueryPayments;

public sealed class QueryPaymentsService(IPaymentRepository paymentRepository)
{
    public Task<int> CountAsync(QueryPaymentsCommand command, CancellationToken cancellationToken)
    {
        return paymentRepository.Count(command.OrderId, cancellationToken);
    }

    public Task<ICollection<Payment>> QueryAsync(QueryPaymentsCommand command, CancellationToken cancellationToken)
    {
        return paymentRepository.Query(
            command.Page,
            command.PageSize,
            command.OrderId,
            cancellationToken);
    }
}

public sealed record QueryPaymentsCommand(int Page, int PageSize, Guid? OrderId);