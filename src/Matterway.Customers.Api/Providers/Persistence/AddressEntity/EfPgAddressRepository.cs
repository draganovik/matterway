using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Providers.Persistence.AddressEntity;

public class EfPgAddressRepository(CustomersDb context) : IAddressRepository
{
    public async Task<Address?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Address.FindAsync([id], cancellationToken);
    }

    public async Task<Address?> GetByCustomerId(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await context.Address.FirstOrDefaultAsync(address => address.CustomerId == customerId,
            cancellationToken);
    }

    public async Task<Address?> Upsert(Address entity, CancellationToken cancellationToken = default)
    {
        var existing = await context.Address.FindAsync([entity.Id], cancellationToken);

        if (existing is null)
            context.Address.Add(entity);
        else
            context.Address.Entry(existing).CurrentValues.SetValues(entity);

        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected < 1) return null;

        return existing ?? entity;
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var affected = await context.Address
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }
}