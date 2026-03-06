using Matterway.Sales.Api.Domain.Entities;

namespace Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;

public interface IPaymentRepository
{
    Task<Payment?> Create(Payment requestModel, CancellationToken cancellationToken = default);

    Task<Payment?> GetById(Guid id, CancellationToken cancellationToken = default);

    Task<ICollection<Payment>> Query(int pageIndex, int pageSize, OrderId? orderId,
        CancellationToken cancellationToken = default);

    Task<int> Count(OrderId? orderId, CancellationToken cancellationToken = default);
}