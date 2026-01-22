using Matterway.Sales.Api.Domain.Entities;

namespace Matterway.Sales.Api.Infrastructure.Persistence.OrderStatusEntity;

public interface IOrderStatusRepository
{
    Task<OrderStatus?> Create(OrderStatus requestModel, CancellationToken cancellationToken = default);
}