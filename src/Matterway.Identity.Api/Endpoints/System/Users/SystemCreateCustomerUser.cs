using System.ComponentModel.DataAnnotations;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Identity.Api.Endpoints.System.Users;

public class SystemCreateCustomerUser : IEndpoint
{
    private const string RouteName = nameof(SystemCreateCustomerUser);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.System, "users/customer", Handler)
            .WithName(RouteName)
            .WithSummary("[system] Create a customer system user")
            .WithTags("SystemUsers")
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireSystemAccessKey()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<CreateUserResponse>, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(
            CreateUserRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            CancellationToken cancellationToken,
            IdentityDbComposer identityDb,
            UserManager<SystemUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
    {
        var strategy = identityDb.Database.CreateExecutionStrategy();
        return await ExecutionStrategyExtensions
            .ExecuteAsync<Results<Created<CreateUserResponse>, BadRequest<ProblemDetails>, ForbidHttpResult>>(
                strategy,
                async () =>
                {
                    await using var transaction = await identityDb.Database.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        var systemUser = new SystemUser
                        {
                            Email = request.Email,
                            UserName = request.Email
                        };

                        var createResult = await userManager.CreateAsync(systemUser, request.Password);
                        if (!createResult.Succeeded)
                            return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                                createResult.Errors.Select(error => error.Description))));

                        var roleResult = await IdentityRoleAdapter.SetPrimaryRoleAsync(userManager, roleManager,
                            systemUser,
                            EIdentityRole.Customer);
                        if (!roleResult.Succeeded)
                            return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                                roleResult.Errors.Select(error => error.Description))));

                        await transaction.CommitAsync(cancellationToken);

                        var location = linkGenerator.GetUriByName(httpContext, "AdminGetSystemUserById",
                            new { id = systemUser.Id });

                        return TypedResults.Created(location, new CreateUserResponse
                        {
                            Id = systemUser.Id,
                            Email = systemUser.Email
                        });
                    }
                    catch
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
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
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class CreateUserResponse
    {
        public Guid Id { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}