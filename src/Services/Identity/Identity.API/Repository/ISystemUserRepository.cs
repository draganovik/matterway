using Identity.API.Entities;
using Identity.API.Models.SystemUserModels;

namespace Identity.API.Repository
{
    public interface ISystemUserRepository
    {
        Task<ICollection<SystemUser>> Query();

        Task<SystemUser?> GetById(Guid id);

        Task<SystemUser?> GetByEmail(string email);

        Task<SystemUser?> GetByCredentials(string email, string password);

        Task<SystemUser?> Create(SystemUser user);

        Task<SystemUser?> Update(Guid id, SystemUserBaseRequestModel user);

        Task<bool> Delete(Guid id);
    }
}
