using Identity.Api.Features.SystemUsers.Contracts;
using Identity.Api.Features.SystemUsers.Domain;

namespace Identity.Api.Features.SystemUsers.Data;

public interface ISystemUserRepository
{
    Task<ICollection<SystemUser>> Query(int pageIndex, int pageSize);

    Task<SystemUser?> GetById(Guid id);

    Task<SystemUser?> GetByEmail(string email);

    Task<SystemUser?> GetByCredentials(string email, string password);

    Task<SystemUser?> Create(SystemUser user);

    Task<SystemUser?> Update(Guid id, SystemUserBaseRequest request);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}