using Matterway.Customers.Api.Domain.Entities;

namespace Matterway.Customers.Api.Infrastructure.Persistence.CustomerOrderEntity;

public interface ICustomerOrderRepository
{
    Task<CustomerOrder?> CreateFromCart(Guid customerId, OrderId orderId,
        CancellationToken cancellationToken = default);

    Task<ICollection<CustomerOrder>> QueryForCustomer(Guid customerId, int pageIndex, int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> CountForCustomer(Guid customerId, CancellationToken cancellationToken = default);
}