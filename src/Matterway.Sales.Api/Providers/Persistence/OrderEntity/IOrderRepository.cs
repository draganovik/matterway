using Matterway.Sales.Api.Domain.Entities;

namespace Matterway.Sales.Api.Providers.Persistence.OrderEntity;

public interface IOrderRepository
{
    Task<Order?> Create(Order requestModel, CancellationToken cancellationToken = default);

    Task<Order?> GetById(Guid id, CancellationToken cancellationToken = default);

    Task<ICollection<Order>> Query(int pageIndex, int pageSize, Guid? customerId,
        CancellationToken cancellationToken = default);

    Task<int> Count(Guid? customerId, CancellationToken cancellationToken = default);

    Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
}