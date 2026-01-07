using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Providers.Persistence.CustomerEntity;

public class EfPgCustomerRepository(CustomersDb context) : ICustomerRepository
{
    public async Task<Customer?> Create(Customer requestModel, CancellationToken cancellationToken = default)
    {
        context.Customer.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1) return await context.Customer.FindAsync([requestModel.Id], cancellationToken);
        return null;
    }

    public async Task<ICollection<Customer>> Query(int pageIndex, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await context.Customer.AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> Update(Customer entity, CancellationToken cancellationToken = default)
    {
        context.Customer.Update(entity);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected == 1) return await context.Customer.FindAsync([entity.Id], cancellationToken);
        return null;
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var affected = await context.Customer
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<Customer?> GetBy(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Customer.FindAsync([id], cancellationToken);
    }

    public async Task<Customer?> GetBySuid(Guid systemUserId, CancellationToken cancellationToken = default)
    {
        return await context.Customer.FirstOrDefaultAsync(model => model.SystemUserId == systemUserId,
            cancellationToken);
    }

    public Task<int> Count(CancellationToken cancellationToken = default)
    {
        return context.Customer.CountAsync(cancellationToken);
    }
}