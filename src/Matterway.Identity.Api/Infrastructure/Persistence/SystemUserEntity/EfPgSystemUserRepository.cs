using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Identity.Api.Infrastructure.Persistence.SystemUserEntity;

public sealed class EfPgSystemUserRepository(IdentityDbComposer context) : ISystemUserRepository
{
    public Task<int> Count(EIdentityRole? roleFilter, CancellationToken cancellationToken = default)
    {
        return ApplyRoleFilter(context.Users.AsNoTracking(), roleFilter)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SystemUserRepositoryModel>> Query(
        int pageIndex,
        int pageSize,
        EIdentityRole? roleFilter,
        CancellationToken cancellationToken = default)
    {
        var users = await ApplyRoleFilter(context.Users.AsNoTracking(), roleFilter)
            .OrderBy(user => user.Created)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (users.Count == 0) return [];

        if (roleFilter.HasValue)
            return users
                .Select(user => MapToReadModel(user, roleFilter.Value))
                .ToList();

        var rolesByUserId = await ResolveRolesByUserIds(
            users.Select(user => user.Id),
            cancellationToken);

        return users
            .Select(user => MapToReadModel(
                user,
                rolesByUserId.GetValueOrDefault(user.Id, EIdentityRole.Customer)))
            .ToList();
    }

    public async Task<SystemUserRepositoryModel?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id, cancellationToken);

        if (user is null) return null;

        var role = await ResolveRoleByUserId(id, cancellationToken);

        return MapToReadModel(user, role);
    }

    private IQueryable<SystemUser> ApplyRoleFilter(IQueryable<SystemUser> users, EIdentityRole? roleFilter)
    {
        if (!roleFilter.HasValue) return users;

        var normalizedRole = roleFilter.Value.ToString().ToUpperInvariant();

        return from user in users
            join userRole in context.UserRoles on user.Id equals userRole.UserId
            join role in context.Roles on userRole.RoleId equals role.Id
            where role.NormalizedName != null && role.NormalizedName == normalizedRole
            select user;
    }

    private async Task<EIdentityRole> ResolveRoleByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var roleName = await (
            from userRole in context.UserRoles
            join role in context.Roles on userRole.RoleId equals role.Id
            where userRole.UserId == userId
            select role.Name).FirstOrDefaultAsync(cancellationToken);

        return ParseRoleName(roleName);
    }

    private async Task<Dictionary<Guid, EIdentityRole>> ResolveRolesByUserIds(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken)
    {
        var distinctIds = userIds.Distinct().ToArray();
        if (distinctIds.Length == 0) return new Dictionary<Guid, EIdentityRole>();

        var roleRows = await (
            from userRole in context.UserRoles
            join role in context.Roles on userRole.RoleId equals role.Id
            where distinctIds.Contains(userRole.UserId)
            select new
            {
                userRole.UserId,
                role.Name
            }).ToListAsync(cancellationToken);

        return roleRows
            .GroupBy(row => row.UserId)
            .ToDictionary(
                group => group.Key,
                group => ParseRoleName(group.Select(item => item.Name).FirstOrDefault())
            );
    }

    private static EIdentityRole ParseRoleName(string? roleName)
    {
        return Enum.TryParse(roleName, true, out EIdentityRole role)
            ? role
            : EIdentityRole.Customer;
    }

    private static SystemUserRepositoryModel MapToReadModel(SystemUser user, EIdentityRole role)
    {
        return new SystemUserRepositoryModel
        {
            Id = user.Id,
            Email = user.Email,
            Created = user.Created,
            Role = role
        };
    }
}