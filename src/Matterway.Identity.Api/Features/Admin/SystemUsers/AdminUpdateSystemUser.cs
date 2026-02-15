using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Identity.Api.Features.Admin.SystemUsers;

public class AdminUpdateSystemUser : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "system-users/{id:guid}", Handler)
            .WithName("AdminUpdateSystemUser").WithSummary("[admin] Update system user by id")
            .WithTags("SystemUsers")
            .Produces<UpdateSystemUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateSystemUserResponse>, BadRequest<ProblemDetails>, NotFound>> Handler(
        Guid id,
        UpdateSystemUserRequest request,
        HttpContext httpContext,
        UserManager<SystemUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var userRole = await IdentityRoleAdapter.GetPrimaryRoleAsync(userManager, user);
        var isAdmin = RequestIdentity.AsAdministrator(httpContext.User);

        if (!isAdmin && userRole != EIdentityRole.Customer)
            return TypedResults.BadRequest(CreateProblemDetails("Operators can only update Customer users"));

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            user.Email = request.Email;
            user.NormalizedEmail = userManager.NormalizeEmail(request.Email);
            user.UserName = request.Email;
            user.NormalizedUserName = userManager.NormalizeName(request.Email);
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
            await userManager.ResetPasswordAsync(user,
                await userManager.GeneratePasswordResetTokenAsync(user), request.Password);

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                updateResult.Errors.Select(error => error.Description))));

        return TypedResults.Ok(new UpdateSystemUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Created = user.Created,
            Role = userRole
        });
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

    public class UpdateSystemUserRequest
    {
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; }
    }

    public class UpdateSystemUserResponse
    {
        public Guid Id { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public DateTime Created { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EIdentityRole Role { get; set; }
    }
}