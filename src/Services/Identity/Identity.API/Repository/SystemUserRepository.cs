using Identity.API.Data;
using Identity.API.Entities;
using Identity.API.Models.SystemUserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Repository;

public class SystemUserRepository : ISystemUserRepository
{
    private readonly IdentityDbContext context;
    private readonly IPasswordHasher<SystemUser> passwordHasher;

    public SystemUserRepository(IdentityDbContext context, IPasswordHasher<SystemUser> passwordHasher)
    {
        this.context = context;
        this.passwordHasher = passwordHasher;
    }

    public async Task<SystemUser?> Create(SystemUser user)
    {
        context.SystemUser.Add(user);
        var affected = await context.SaveChangesAsync();
        if (affected == 1) return await context.SystemUser.FindAsync(user.Id);
        return null;
    }

    public async Task<bool> Delete(Guid id)
    {
        var affected = await context.SystemUser
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<SystemUser?> GetByCredentials(string email, string password)
    {
        var user = await context.SystemUser
            .SingleOrDefaultAsync(u => u.Email == email);
        if (user is null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, password) !=
            PasswordVerificationResult.Success) return null;
        return user;
    }

    public async Task<SystemUser?> GetByEmail(string email)
    {
        return await context.SystemUser.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<SystemUser?> GetById(Guid id)
    {
        return await context.SystemUser.FindAsync(id);
    }

    public Task<int> GetTotalEntities()
    {
        return context.SystemUser.CountAsync();
    }

    public async Task<ICollection<SystemUser>> Query(int pageIndex, int pageSize)
    {
        return await context.SystemUser.AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<SystemUser?> Update(Guid id, SystemUserBaseRequestModel user)
    {
        var currentUserModel = await context.SystemUser.FindAsync(id);
        if (currentUserModel is null) return null;
        var affected = await context.SystemUser
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Email, user.Email)
                .SetProperty(m => m.Role, user.Role)
                .SetProperty(m => m.PasswordHash, passwordHasher.HashPassword(currentUserModel, user.Password!))
            );
        await context.Entry(currentUserModel).ReloadAsync();
        return affected == 1 ? currentUserModel : null;
    }
}