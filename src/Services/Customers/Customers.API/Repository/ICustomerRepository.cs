using Customers.API.Entities;
using Customers.API.Models.CustomerModels;

namespace Customers.API.Repository
{
    public interface ICustomerRepository
    {
        Task<ICollection<Customer>> Query();

        Task<Customer?> GetById(Guid id);

        Task<Customer?> Create(Customer customer);

        Task<Customer?> Update(Guid id, CustomerUpdateRequestModel customer);

        Task<bool> Delete(Guid id);
    }
}
