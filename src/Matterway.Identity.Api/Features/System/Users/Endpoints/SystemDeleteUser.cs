using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Features.System.Users.Endpoints;

public class SystemDeleteUser : IEndpoint
{
    private const string RouteName = nameof(SystemDeleteUser);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.System, "users/{id:guid}", Handler)
            .WithName(RouteName)
            .WithSummary("[system] Delete a system user by id")
            .WithTags("SystemUsers")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireSystemAccessKey()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<NoContent, NotFound, ProblemHttpResult>> Handler(
        Guid id,
        UserManager<SystemUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var deleteResult = await userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Unable to delete system user",
                Status = StatusCodes.Status500InternalServerError,
                Detail = string.Join("; ", deleteResult.Errors.Select(error => error.Description))
            });

        return TypedResults.NoContent();
    }
}