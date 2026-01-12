using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Domain;

public static class IdentityRoleAdapter
{
    public static async Task<EIdentityRole> GetPrimaryRoleAsync(
        UserManager<SystemUser> userManager,
        SystemUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        if (roles.Count == 0) return EIdentityRole.Customer;

        return Enum.TryParse<EIdentityRole>(roles[0], out var role)
            ? role
            : EIdentityRole.Customer;
    }

    public static async Task<IdentityResult> SetPrimaryRoleAsync(
        UserManager<SystemUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        SystemUser user,
        EIdentityRole role)
    {
        var roleName = role.ToString();
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var createRoleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            if (!createRoleResult.Succeeded) return createRoleResult;
        }

        var existingRoles = await userManager.GetRolesAsync(user);
        if (existingRoles.Count > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, existingRoles);
            if (!removeResult.Succeeded) return removeResult;
        }

        return await userManager.AddToRoleAsync(user, roleName);
    }
}