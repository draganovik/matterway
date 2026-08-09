using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Features.Admin.UserPerms.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Features.Admin.UserPerms.Endpoints;

public class AdminGetSystemUserPerms : IEndpoint
{
    private const string RouteName = nameof(AdminGetSystemUserPerms);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "system-users/{id:guid}/perms", Handler)
            .WithName(RouteName)
            .WithSummary("[admin] Get permissions for a system user")
            .WithTags("SystemUsers")
            .Produces<IEnumerable<AdminGetSystemUserPermResponse>>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<IEnumerable<AdminGetSystemUserPermResponse>>, NotFound>> Handler(
        Guid id,
        UserManager<SystemUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var role = await IdentityRoleAdapter.GetPrimaryRoleAsync(userManager, user);
        var response = role == EIdentityRole.Employee
            ? (await IdentityPermissionAdapter.GetServicePermissionsAsync(userManager, user))
            .Select(permission => new AdminGetSystemUserPermResponse
            {
                Service = permission.Service,
                Level = permission.Level
            })
            : Enumerable.Empty<AdminGetSystemUserPermResponse>();

        return TypedResults.Ok(response);
    }
}