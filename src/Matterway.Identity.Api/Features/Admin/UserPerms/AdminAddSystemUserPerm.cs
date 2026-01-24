using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Matterway.Identity.Api.Application;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.ServiceDefaults.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Identity.Api.Features.Admin.UserPerms;

public class AdminAddSystemUserPerm : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("admin/system-users/{id:guid}/perms", Handler)
            .WithName("AdminAddSystemUserPerm")
            .WithSummary("Add a permission to a system user.")
            .WithTags("SystemUsers")
            .Produces<IEnumerable<AddSystemUserPermResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsAdministrator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async
        Task<Results<Ok<IEnumerable<AddSystemUserPermResponse>>, BadRequest<ProblemDetails>, NotFound>>
        Handler(
            Guid id,
            AddSystemUserPermRequest request,
            UserManager<SystemUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var userRole = await IdentityRoleAdapter.GetPrimaryRoleAsync(userManager, user);
        if (userRole != EIdentityRole.Employee)
            return TypedResults.BadRequest(CreateProblemDetails("Permissions can only be assigned to employees."));

        if (string.IsNullOrWhiteSpace(request.Service) || request.Level is null)
            return TypedResults.BadRequest(CreateProblemDetails("Service and level are required."));

        var permissionValue = PermissionClaims.Format(request.Service, request.Level.Value);
        var claims = await userManager.GetClaimsAsync(user);
        var existingClaim = claims.FirstOrDefault(existing =>
            string.Equals(existing.Type, PermissionClaims.ClaimType, StringComparison.Ordinal) &&
            string.Equals(existing.Value, permissionValue, StringComparison.OrdinalIgnoreCase));

        if (existingClaim is null)
        {
            var claim = new Claim(PermissionClaims.ClaimType, permissionValue);
            var addResult = await userManager.AddClaimAsync(user, claim);
            if (!addResult.Succeeded)
                return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                    addResult.Errors.Select(error => error.Description))));
        }

        var updatedClaims = await userManager.GetClaimsAsync(user);
        var response = updatedClaims
            .Where(existing => string.Equals(existing.Type, PermissionClaims.ClaimType, StringComparison.Ordinal))
            .Select(existing => PermissionClaims.TryParse(existing.Value, out var service, out var level)
                ? new AddSystemUserPermResponse
                {
                    Service = service,
                    Level = level
                }
                : null)
            .Where(responseItem => responseItem is not null)
            .Select(responseItem => responseItem!);

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

    public class AddSystemUserPermRequest
    {
        [Required(ErrorMessage = "Service is required.")]
        public string? Service { get; set; }

        [Required(ErrorMessage = "Permission level is required.")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PermissionLevel? Level { get; set; }
    }

    public class AddSystemUserPermResponse
    {
        public string Service { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PermissionLevel Level { get; set; }
    }
}