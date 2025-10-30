using Matterway.Ordering.Api.Features.Addresses.Contracts;
using Matterway.Ordering.Api.Features.Addresses.Domain;

namespace Matterway.Ordering.Api.Features.Addresses.Data;

public interface IAddressRepository
{
    Task<ICollection<Address>> Query(int pageIndex, int pageSize);

    Task<Address?> GetById(Guid id);

    Task<Address?> Create(Address requestModel);

    Task<Address?> Update(Guid id, AddressBaseRequest requestModel);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}