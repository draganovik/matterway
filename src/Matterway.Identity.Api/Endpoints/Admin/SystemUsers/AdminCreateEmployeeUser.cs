using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Endpoints.Admin.SystemUsers;

public class AdminCreateEmployeeUser : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "users/employee", Handler)
            .WithName("AdminCreateEmployeeUser")
            .WithSummary("[admin] Create an Employee user")
            .WithTags("SystemUsers")
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<CreateUserResponse>, BadRequest<ProblemDetails>>> Handler(
        CreateUserRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        CancellationToken cancellationToken,
        IdentityDbComposer identityDb,
        UserManager<SystemUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        await using var transaction = await identityDb.Database.BeginTransactionAsync(cancellationToken);

        var systemUser = new SystemUser
        {
            Email = request.Email,
            UserName = request.Email
        };

        var createResult = await userManager.CreateAsync(systemUser, request.Password);
        if (!createResult.Succeeded)
            return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                createResult.Errors.Select(error => error.Description))));

        var roleResult = await IdentityRoleAdapter.SetPrimaryRoleAsync(userManager, roleManager, systemUser,
            EIdentityRole.Employee);
        if (!roleResult.Succeeded)
            return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                roleResult.Errors.Select(error => error.Description))));

        var permissionResult = await IdentityPermissionAdapter.EnsureEmployeeObserverDefaultsAsync(
            userManager,
            systemUser);

        if (!permissionResult.Succeeded)
            return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                permissionResult.Errors.Select(error => error.Description))));

        await transaction.CommitAsync(cancellationToken);

        var location = linkGenerator.GetUriByName(httpContext, "AdminGetSystemUserById",
            new { id = systemUser.Id });

        return TypedResults.Created(location, new CreateUserResponse
        {
            Id = systemUser.Id,
            Email = systemUser.Email,
            Created = systemUser.Created,
            Role = EIdentityRole.Employee
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

    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; init; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; init; }
    }

    public class CreateUserResponse
    {
        public Guid Id { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public DateTime Created { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EIdentityRole Role { get; set; }
    }
}