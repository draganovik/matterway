using Asp.Versioning;
using Matterway.Identity.Api.Application;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Features.Admin.SystemUsers;

public class AdminDeleteSystemUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("admin/system-users/{id:guid}", Handler)
            .WithName("AdminDeleteSystemUser").WithSummary("Delete system user by id (admin).")
            .WithTags("SystemUsers")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsAdministrator(context.User)))
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