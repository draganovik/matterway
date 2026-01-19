using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Matterway.Identity.Api.Application;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Identity.Api.Features.Users;

public class CreateSystemUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("SystemUsers", Handler)
            .WithName("CreateSystemUser").WithSummary("Create a new system user.")
            .WithTags("SystemUsers")
            .Produces<CreateSystemUserResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status403Forbidden)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<CreateSystemUserResponse>, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(
            CreateSystemUserRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            UserManager<SystemUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
    {
        var requestIdentityExists = RequestIdentity
            .TryGet(httpContext.User, out var requestIdentity);

        if (!requestIdentityExists && request.Role != EIdentityRole.Customer)
            return TypedResults.Forbid();

        if (requestIdentityExists && requestIdentity.Role != EIdentityRole.Admin &&
            request.Role != EIdentityRole.Customer)
            return TypedResults.BadRequest(CreateProblemDetails("Only Admin can create non-Customer users"));

        var systemUser = new SystemUser
        {
            Email = request.Email,
            UserName = request.Email
        };

        var createResult = await userManager.CreateAsync(systemUser, request.Password);
        if (!createResult.Succeeded)
            return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                createResult.Errors.Select(error => error.Description))));

        var role = request.Role ?? EIdentityRole.Customer;
        var roleResult = await IdentityRoleAdapter.SetPrimaryRoleAsync(userManager, roleManager, systemUser, role);
        if (!roleResult.Succeeded)
            return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                roleResult.Errors.Select(error => error.Description))));

        var location = linkGenerator.GetPathByName(httpContext, "GetSystemUserById",
            new { id = systemUser.Id });

        var response = new CreateSystemUserResponse
        {
            Id = systemUser.Id,
            Email = systemUser.Email,
            Created = systemUser.Created,
            Role = role
        };

        return TypedResults.Created(location, response);
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

    public class CreateSystemUserRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; init; }

        [PasswordPropertyText]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EIdentityRole? Role { get; set; }
    }

    public class CreateSystemUserResponse
    {
        public Guid Id { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public DateTime Created { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EIdentityRole Role { get; set; }
    }
}