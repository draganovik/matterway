using Customers.API.Data;
using Customers.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customers.API.Repository;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomersDbContext context;

    public CustomerRepository(CustomersDbContext context)
    {
        this.context = context;
    }

    public async Task<Customer?> Create(Customer requestModel)
    {
        context.Customer.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1) return await context.Customer.FindAsync(requestModel.Id);
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

    public async Task<Customer?> GetBySystemUserId(Guid id)
    {
        return await context.Customer.FirstOrDefaultAsync(model => model.SystemUserId == id);
    }

    public Task<int> GetTotalEntities()
    {
        return context.Customer.CountAsync();
    }

    public async Task<ICollection<Customer>> Query(int pageIndex, int pageSize)
    {
        return await context.Customer.AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Customer?> Update(Guid id, Customer requestModel)
    {
        var currentCustomerModel = await context.Customer.FindAsync(id);
        if (currentCustomerModel is null) return null;
        var affected = await context.Customer
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.SystemUserId, requestModel.SystemUserId)
                .SetProperty(m => m.FirstName, requestModel.FirstName)
                .SetProperty(m => m.LastName, requestModel.LastName)
                .SetProperty(m => m.BirthDate, requestModel.BirthDate)
                .SetProperty(m => m.DefaultAddressId, requestModel.DefaultAddressId)
            );
        await context.Entry(currentCustomerModel).ReloadAsync();
        return affected == 1 ? currentCustomerModel : null;
    }
}