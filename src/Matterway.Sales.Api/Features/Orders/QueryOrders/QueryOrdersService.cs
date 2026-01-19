using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

namespace Matterway.Sales.Api.Features.Orders.QueryOrders;

public sealed class QueryOrdersService(IOrderRepository orderRepository)
{
    public Task<int> CountAsync(QueryOrdersCommand command, CancellationToken cancellationToken)
    {
        return orderRepository.Count(command.CustomerId, cancellationToken);
    }

    public Task<ICollection<Order>> QueryAsync(QueryOrdersCommand command, CancellationToken cancellationToken)
    {
        return orderRepository.Query(
            command.Page,
            command.PageSize,
            command.CustomerId,
            cancellationToken);
    }
}

public sealed record QueryOrdersCommand(int Page, int PageSize, Guid? CustomerId);