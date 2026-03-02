using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Endpoints.Admin.SystemUsers;

public class AdminDeleteSystemUser : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "system-users/{id:guid}", Handler)
            .WithName("AdminDeleteSystemUser").WithSummary("[admin] Delete system user by id")
            .WithTags("SystemUsers")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound>> Handler(
        Guid id,
        UserManager<SystemUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}