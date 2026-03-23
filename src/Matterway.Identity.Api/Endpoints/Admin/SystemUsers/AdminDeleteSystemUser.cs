using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Endpoints.Admin.SystemUsers;

public class AdminDeleteSystemUser : IEndpoint
{
    private const string RouteName = nameof(AdminDeleteSystemUser);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "system-users/{id:guid}", Handler)
            .WithName(RouteName).WithSummary("[admin] Delete system user by id")
            .WithTags("SystemUsers")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<NoContent, NotFound, ProblemHttpResult>> Handler(
        Guid id,
        HttpContext httpContext,
        UserManager<SystemUser> userManager)
    {
        var requesterId = RequestIdentity.GetIdentifier(httpContext.User);
        if (requesterId == id)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Action forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = "Managers cannot delete their own account."
            });

        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}