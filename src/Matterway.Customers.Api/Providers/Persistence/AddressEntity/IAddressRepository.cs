using Matterway.Customers.Api.Domain.Entities;

namespace Matterway.Customers.Api.Providers.Persistence.AddressEntity;

public interface IAddressRepository
{
    Task<Address?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<Address?> GetByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<Address?> Upsert(Address entity, CancellationToken cancellationToken = default);
    Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);
}