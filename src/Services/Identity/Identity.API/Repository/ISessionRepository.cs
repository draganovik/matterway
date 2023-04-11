using Identity.API.Entities;

namespace Identity.API.Repository;

public interface ISessionRepository
{
    Task<ICollection<Session>> Query(int pageIndex, int pageSize);

    Task<Session?> GetById(Guid id);

    Task<Session?> GetByToken(string token);

    Task<Session?> GetByRefreshToken(string refreshToken);

    Task<Session?> Create(Session session);

    Task<Session?> Refresh(Session session);

    Task<bool> DeleteByToken(string token);

    Task<bool> DeleteByRefreshToken(string refreshToken);
}
