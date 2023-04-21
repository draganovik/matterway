using Identity.API.Data;
using Identity.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Repository;

public class SessionRepository : ISessionRepository
{
    private readonly IdentityDbContext context;
    public SessionRepository(IdentityDbContext context)
    {
        this.context = context;
    }
    public async Task<ICollection<Session>> Query(int pageIndex, int pageSize)
    {
        return await context.Session.Include(s => s.SystemUser)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    public async Task<Session?> GetById(Guid id)
    {
        return await context.Session.Include(s => s.SystemUser).FirstOrDefaultAsync(s => s.Id == id);
    }
    public async Task<Session?> GetByToken(string token)
    {
        return await context.Session.Include(s => s.SystemUser).FirstOrDefaultAsync(s => s.Token == token);
    }
    public async Task<Session?> GetByRefreshToken(string refreshToken)
    {
        return await context.Session.Include(s => s.SystemUser).FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);
    }
    public async Task<Session?> Create(Session session)
    {
        await context.Session.AddAsync(session);
        await context.SaveChangesAsync();
        return session;
    }
    public async Task<Session?> Refresh(Session session)
    {
        context.Session.Update(session);
        await context.SaveChangesAsync();
        return session;
    }
    public async Task<bool> DeleteByToken(string token)
    {
        var session = await context.Session.FirstOrDefaultAsync(s => s.Token == token);
        if (session == null)
        {
            return false;
        }

        context.Session.Remove(session);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByRefreshToken(string refreshToken)
    {
        var session = await context.Session.FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);
        if (session == null)
        {
            return false;
        }

        context.Session.Remove(session);
        await context.SaveChangesAsync();
        return true;
    }

    public Task<int> GetTotalEntities()
    {
        return context.Session.CountAsync();
    }
}
