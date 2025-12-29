using Matterway.Customers.Api.Domain.Entities;

namespace Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

public interface ICustomerRepository
{
    Task<Customer?> Create(Customer requestModel);

    Task<ICollection<Customer>> Query(int pageIndex, int pageSize);

    Task<Customer?> Update(Customer entity);

    Task<bool> Delete(Guid id);

    Task<Customer?> GetById(Guid id);

    Task<Customer?> GetBySystemUserId(Guid systemUserId);

    Task<int> Count();
}