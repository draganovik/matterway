using Matterway.Sales.Api.Domain.Entities;

namespace Matterway.Sales.Api.Providers.Persistence.PaymentEntity;

public interface IPaymentRepository
{
    Task<Payment?> Create(Payment requestModel, CancellationToken cancellationToken = default);

    Task<Payment?> GetById(Guid id, CancellationToken cancellationToken = default);

    Task<ICollection<Payment>> Query(int pageIndex, int pageSize, Guid? orderId,
        CancellationToken cancellationToken = default);

    Task<int> Count(Guid? orderId, CancellationToken cancellationToken = default);
}