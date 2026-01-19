using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

namespace Matterway.Sales.Api.Features.Orders.GetOrderById;

public sealed class GetOrderByIdService(IOrderRepository orderRepository)
{
    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return orderRepository.GetById(orderId, cancellationToken);
    }
}