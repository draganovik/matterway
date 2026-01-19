using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderStatusEntity;

namespace Matterway.Sales.Api.Features.Orders.AddOrderStatus;

public sealed class AddOrderStatusService(
    IOrderRepository orderRepository,
    IOrderStatusRepository orderStatusRepository)
{
    public Task<bool> OrderExistsAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return orderRepository.Exists(orderId, cancellationToken);
    }

    public OrderStatus BuildStatus(AddOrderStatusCommand command)
    {
        return new OrderStatus
        {
            OrderId = command.OrderId,
            Status = command.Status,
            Note = command.Note,
            ChangedAt = DateTime.UtcNow
        };
    }

    public Task<OrderStatus?> PersistAsync(OrderStatus status, CancellationToken cancellationToken)
    {
        return orderStatusRepository.Create(status, cancellationToken);
    }
}

public sealed record AddOrderStatusCommand(Guid OrderId, EOrderStatusType Status, string? Note);