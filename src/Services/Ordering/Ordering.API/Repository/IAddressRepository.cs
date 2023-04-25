using Ordering.API.Entities;
using Ordering.API.Models.AddressModels;

namespace Ordering.API.Repository;

public interface IAddressRepository
{
    Task<ICollection<Address>> Query(int pageIndex, int pageSize);

    Task<Address?> GetById(Guid id);

    Task<Address?> Create(Address requestModel);

    Task<Address?> Update(Guid id, AddressBaseRequestModel requestModel);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();

}
