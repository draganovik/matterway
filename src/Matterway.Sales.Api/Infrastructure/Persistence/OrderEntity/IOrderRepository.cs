using Matterway.Sales.Api.Domain.Entities;

namespace Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

public interface IOrderRepository
{
    Task<Order?> Create(Order requestModel, CancellationToken cancellationToken = default);

    Task<Order?> Update(Order requestModel, CancellationToken cancellationToken = default);

    Task<Order?> GetById(OrderId id, CancellationToken cancellationToken = default);

    Task<Order?> GetPendingByCustomerId(Guid customerId, CancellationToken cancellationToken = default);

    Task<ICollection<Order>> Query(int pageIndex, int pageSize, Guid? customerId,
        CancellationToken cancellationToken = default);

    Task<int> Count(Guid? customerId, CancellationToken cancellationToken = default);

    Task<bool> Exists(OrderId id, CancellationToken cancellationToken = default);
}