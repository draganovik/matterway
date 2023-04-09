using Customers.API.Data;
using Customers.API.Entities;
using Customers.API.Models.CustomerModels;
using Microsoft.EntityFrameworkCore;

namespace Customers.API.Repository;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomersDbContext context;

    public CustomerRepository(CustomersDbContext context)
    {
        this.context = context;
    }

    public async Task<Customer?> Create(Customer customer)
    {
        context.Customer.Add(customer);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.Customer.FindAsync(customer.Id);
        }
        return null;
    }

    public async Task<bool> Delete(Guid id)
    {
        var affected = await context.Customer
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<Customer?> GetById(Guid id)
    {
        return await context.Customer.FindAsync(id);
    }

    public async Task<ICollection<Customer>> Query()
    {
        return await context.Customer.AsNoTracking()
        .ToListAsync();
    }

    public async Task<Customer?> Update(Guid id, CustomerUpdateRequestModel customer)
    {
        var currentUserModel = await context.Customer.FindAsync(id);
        if (currentUserModel is null)
        {
            return null;
        }
        var affected = await context.Customer
        .Where(model => model.Id == id)
        .ExecuteUpdateAsync(setters => setters
              .SetProperty(m => m.SystemUserId, customer.SystemUserId)
              .SetProperty(m => m.FirstName, customer.FirstName)
              .SetProperty(m => m.LastName, customer.LastName)
              .SetProperty(m => m.BirthDate, customer.BirthDate)
              .SetProperty(m => m.DefaultAddressId, customer.DefaultAddressId)
            );
        await context.Entry(currentUserModel).ReloadAsync();
        return affected == 1 ? currentUserModel : null;
    }
}
