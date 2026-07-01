using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Features.Admin.UserPerms.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Features.Admin.UserPerms.Endpoints;

public class AdminPatchSystemUserPerm : IEndpoint
{
    private const string RouteName = nameof(AdminPatchSystemUserPerm);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "system-users/{id:guid}/perms", Handler)
            .WithName(RouteName)
            .WithSummary("[admin] Set a permission for a system user")
            .WithTags("SystemUsers")
            .Produces<IEnumerable<AdminPatchSystemUserPermResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async
        Task<Results<Ok<IEnumerable<AdminPatchSystemUserPermResponse>>, BadRequest<ProblemDetails>, NotFound>>
        Handler(
            Guid id,
            AdminPatchSystemUserPermRequest request,
            UserManager<SystemUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var userRole = await IdentityRoleAdapter.GetPrimaryRoleAsync(userManager, user);
        if (userRole != EIdentityRole.Employee)
            return TypedResults.BadRequest(CreateProblemDetails("Permissions can only be assigned to employees."));

        if (string.IsNullOrWhiteSpace(request.Service) || request.Level is null)
            return TypedResults.BadRequest(CreateProblemDetails("Service and level are required."));

        var setResult = await IdentityPermissionAdapter.SetServicePermissionAsync(
            userManager,
            user,
            request.Service,
            request.Level.Value);

        if (!setResult.Succeeded)
            return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                setResult.Errors.Select(error => error.Description))));

        var response = (await IdentityPermissionAdapter.GetServicePermissionsAsync(userManager, user))
            .Select(permission => new AdminPatchSystemUserPermResponse
            {
                Service = permission.Service,
                Level = permission.Level
            });

        return TypedResults.Ok(response);
    }

    private static ProblemDetails CreateProblemDetails(string detail)
    {
        return new ProblemDetails
        {
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = detail
        };
    }
}