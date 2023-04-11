using Customers.API.Entities;
using Customers.API.Models.CustomerModels;

namespace Customers.API.Repository;

public interface ICustomerRepository
{
    Task<ICollection<Customer>> Query(int pageIndex, int pageSize);

    Task<Customer?> GetById(Guid id);

    Task<Customer?> GetBySystemUserId(Guid id);

    Task<Customer?> Create(Customer requestModel);

    Task<Customer?> Update(Guid id, CustomerUpdateRequestModel requestModel);

    Task<bool> Delete(Guid id);
}
