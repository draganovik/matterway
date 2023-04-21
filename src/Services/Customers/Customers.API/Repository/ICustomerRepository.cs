using Customers.API.Entities;

namespace Customers.API.Repository;

public interface ICustomerRepository
{
    Task<ICollection<Customer>> Query(int pageIndex, int pageSize);

    Task<Customer?> GetById(Guid id);

    Task<Customer?> GetBySystemUserId(Guid id);

    Task<Customer?> Create(Customer requestModel);

    Task<Customer?> Update(Guid id, Customer requestModel);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}
