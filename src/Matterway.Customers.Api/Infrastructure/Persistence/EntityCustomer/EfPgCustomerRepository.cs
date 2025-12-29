using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Infrastructure.Persistence.EntityCustomer;

public class EfPgCustomerRepository(CustomersDb context) : ICustomerRepository
{
    public async Task<Customer?> Create(Customer requestModel)
    {
        context.Customer.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1) return await context.Customer.FindAsync(requestModel.Id);
        return null;
    }

    public async Task<ICollection<Customer>> Query(int pageIndex, int pageSize)
    {
        return await context.Customer.AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Customer?> Update(Customer entity)
    {
        context.Customer.Update(entity);
        var affected = await context.SaveChangesAsync();
        if (affected == 1) return await context.Customer.FindAsync(entity.Id);
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

    public async Task<Customer?> GetBySystemUserId(Guid systemUserId)
    {
        return await context.Customer.FirstOrDefaultAsync(model => model.SystemUserId == systemUserId);
    }

    public Task<int> Count()
    {
        return context.Customer.CountAsync();
    }
}