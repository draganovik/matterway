using Microsoft.EntityFrameworkCore;
using Ordering.Api.Data;
using Ordering.Api.Features.Addresses.Contracts;
using Ordering.Api.Features.Addresses.Domain;

namespace Ordering.Api.Features.Addresses.Data;

public class AddressRepository : IAddressRepository
{
    private readonly OrderingDbContext context;

    public AddressRepository(OrderingDbContext context)
    {
        this.context = context;
    }

    public async Task<Address?> Create(Address requestModel)
    {
        context.Address.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1) return await context.Address.FindAsync(requestModel.Id);
        return null;
    }

    public async Task<bool> Delete(Guid id)
    {
        var affected = await context.Address
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<Address?> GetById(Guid id)
    {
        return await context.Address.FindAsync(id);
    }

    public async Task<int> GetTotalEntities()
    {
        return await context.Address.CountAsync();
    }

    public async Task<ICollection<Address>> Query(int pageIndex, int pageSize)
    {
        return await context.Address.AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Address?> Update(Guid id, AddressBaseRequest requestModel)
    {
        var currentAddressModel = await context.Address.FindAsync(id);
        if (currentAddressModel is null) return null;
        var affected = await context.Address
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.ReceiverName, requestModel.ReceiverName)
                .SetProperty(m => m.Residence, requestModel.Residence)
                .SetProperty(m => m.Street, requestModel.Street)
                .SetProperty(m => m.City, requestModel.City)
                .SetProperty(m => m.ZipCode, requestModel.ZipCode)
                .SetProperty(m => m.Note, requestModel.Note)
            );
        await context.Entry(currentAddressModel).ReloadAsync();
        return affected == 1 ? currentAddressModel : null;
    }
}