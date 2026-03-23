using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Endpoints.Admin.UserPerms;

public class AdminPatchSystemUserPerm : IEndpoint
{
    private const string RouteName = nameof(AdminPatchSystemUserPerm);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "system-users/{id:guid}/perms", Handler)
            .WithName(RouteName)
            .WithSummary("[admin] Set a permission for a system user")
            .WithTags("SystemUsers")
            .Produces<IEnumerable<PatchSystemUserPermResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async
        Task<Results<Ok<IEnumerable<PatchSystemUserPermResponse>>, BadRequest<ProblemDetails>, NotFound>>
        Handler(
            Guid id,
            PatchSystemUserPermRequest request,
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
            .Select(permission => new PatchSystemUserPermResponse
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

    public class PatchSystemUserPermRequest
    {
        [Required(ErrorMessage = "Service is required.")]
        public string? Service { get; set; }

        [Required(ErrorMessage = "Permission level is required.")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PermissionLevel? Level { get; set; }
    }

    public class PatchSystemUserPermResponse
    {
        public string Service { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PermissionLevel Level { get; set; }
    }
}