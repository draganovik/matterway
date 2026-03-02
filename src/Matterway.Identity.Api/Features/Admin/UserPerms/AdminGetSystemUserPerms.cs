using System.Text.Json.Serialization;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Features.Admin.UserPerms;

public class AdminGetSystemUserPerms : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "system-users/{id:guid}/perms", Handler)
            .WithName("AdminGetSystemUserPerms")
            .WithSummary("[admin] Get permissions for a system user")
            .WithTags("SystemUsers")
            .Produces<IEnumerable<GetSystemUserPermResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<IEnumerable<GetSystemUserPermResponse>>, NotFound>> Handler(
        Guid id,
        UserManager<SystemUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var role = await IdentityRoleAdapter.GetPrimaryRoleAsync(userManager, user);
        var response = role == EIdentityRole.Employee
            ? (await IdentityPermissionAdapter.GetServicePermissionsAsync(userManager, user))
            .Select(permission => new GetSystemUserPermResponse
            {
                Service = permission.Service,
                Level = permission.Level
            })
            : Enumerable.Empty<GetSystemUserPermResponse>();

        return TypedResults.Ok(response);
    }

    public class GetSystemUserPermResponse
    {
        public string Service { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PermissionLevel Level { get; set; }
    }
}