using Matterway.Customers.Api.Features.Customers.Contracts;
using Matterway.Customers.Api.Features.Customers.Domain;

namespace Matterway.Customers.Api.Features.Customers.Data;

public interface ICustomerRepository
{
    Task<ICollection<Customer>> Query(int pageIndex, int pageSize);

    Task<Customer?> GetById(Guid id);

    Task<Customer?> GetBySystemUserId(Guid systemUserId);

    Task<Customer?> Create(Customer requestModel);

    Task<Customer?> Update(Guid id, CustomerBaseRequest request);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}