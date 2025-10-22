using Identity.Api.Data;
using Identity.Api.Features.Sessions.Domain;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Features.Sessions.Data;

public class SessionRepository : ISessionRepository
{
    private readonly IdentityDbContext _context;

    public SessionRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Session>> Query(int pageIndex, int pageSize)
    {
        return await _context.Session
            .Include(s => s.SystemUser)
            .OrderByDescending(s => s.Created)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<Session?> GetById(Guid id)
    {
        return _context.Session
            .Include(s => s.SystemUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public Task<Session?> GetByToken(string token)
    {
        return _context.Session
            .Include(s => s.SystemUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Token == token);
    }

    public Task<Session?> GetByRefreshToken(string refreshToken)
    {
        return _context.Session
            .Include(s => s.SystemUser)
            .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);
    }

    public async Task<Session?> Create(Session session)
    {
        await _context.Session.AddAsync(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<Session?> Refresh(Session session)
    {
        _context.Session.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<bool> DeleteByToken(string token)
    {
        var affected = await _context.Session
            .Where(s => s.Token == token)
            .ExecuteDeleteAsync();

        return affected == 1;
    }

    public async Task<bool> DeleteByRefreshToken(string refreshToken)
    {
        var affected = await _context.Session
            .Where(s => s.RefreshToken == refreshToken)
            .ExecuteDeleteAsync();

        return affected == 1;
    }

    public Task<int> GetTotalEntities()
    {
        return _context.Session.CountAsync();
    }
}