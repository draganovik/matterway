using Matterway.Customers.Api.Domain.Entities;

namespace Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

public interface ICustomerRepository
{
    Task<Customer?> Create(Customer requestModel, CancellationToken cancellationToken = default);

    Task<ICollection<Customer>> Query(int pageIndex, int pageSize, CancellationToken cancellationToken = default);

    Task<Customer?> Update(Customer entity, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);

    Task<Customer?> GetBy(Guid id, CancellationToken cancellationToken = default);

    Task<Customer?> GetBySuid(Guid systemUserId, CancellationToken cancellationToken = default);

    Task<int> Count(CancellationToken cancellationToken = default);
}