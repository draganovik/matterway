using Matterway.Identity.Api.Data;
using Matterway.Identity.Api.Features.SystemUsers.Contracts;
using Matterway.Identity.Api.Features.SystemUsers.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Identity.Api.Features.SystemUsers.Data;

public class SystemUserRepository : ISystemUserRepository
{
    private readonly IdentityDb _context;
    private readonly IPasswordHasher<SystemUser> _passwordHasher;

    public SystemUserRepository(IdentityDb context, IPasswordHasher<SystemUser> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<SystemUser?> Create(SystemUser user)
    {
        _context.SystemUser.Add(user);
        var affected = await _context.SaveChangesAsync();

        return affected == 1 ? await _context.SystemUser.FindAsync(user.Id) : null;
    }

    public async Task<bool> Delete(Guid id)
    {
        var affected = await _context.SystemUser
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();

        return affected == 1;
    }

    public async Task<SystemUser?> GetByCredentials(string email, string password)
    {
        var user = await _context.SystemUser.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null) return null;

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, password);
        return verification == PasswordVerificationResult.Success ? user : null;
    }

    public async Task<SystemUser?> GetByEmail(string email)
    {
        return await _context.SystemUser.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
    }

    public Task<SystemUser?> GetById(Guid id)
    {
        return _context.SystemUser.FindAsync(id).AsTask();
    }

    public Task<int> GetTotalEntities()
    {
        return _context.SystemUser.CountAsync();
    }

    public async Task<ICollection<SystemUser>> Query(int pageIndex, int pageSize)
    {
        return await _context.SystemUser.AsNoTracking()
            .OrderByDescending(u => u.Created)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<SystemUser?> Update(Guid id, SystemUserBaseRequest request)
    {
        var currentUser = await _context.SystemUser.FindAsync(id);
        if (currentUser is null) return null;

        var affected = await _context.SystemUser
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Email, request.Email)
                .SetProperty(m => m.Role, request.Role)
                .SetProperty(m => m.PasswordHash,
                    _passwordHasher.HashPassword(currentUser, request.Password!))
            );

        await _context.Entry(currentUser).ReloadAsync();

        return affected == 1 ? currentUser : null;
    }
}