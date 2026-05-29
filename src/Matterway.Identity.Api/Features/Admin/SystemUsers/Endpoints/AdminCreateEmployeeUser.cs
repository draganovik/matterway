using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Features.Admin.SystemUsers.Contracts;
using Matterway.Identity.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Identity.Api.Features.Admin.SystemUsers.Endpoints;

public class AdminCreateEmployeeUser : IEndpoint
{
    private const string RouteName = nameof(AdminCreateEmployeeUser);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "users/employee", Handler)
            .WithName(RouteName)
            .WithSummary("[admin] Create an Employee user")
            .WithTags("SystemUsers")
            .Produces<AdminCreateEmployeeSystemUserResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<AdminCreateEmployeeSystemUserResponse>, BadRequest<ProblemDetails>>>
        Handler(
            AdminCreateEmployeeSystemUserRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            CancellationToken cancellationToken,
            IdentityDbComposer identityDb,
            UserManager<SystemUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
    {
        var strategy = identityDb.Database.CreateExecutionStrategy();
        return await ExecutionStrategyExtensions
            .ExecuteAsync<Results<Created<AdminCreateEmployeeSystemUserResponse>, BadRequest<ProblemDetails>>>(
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

                        return TypedResults.Created(location, new AdminCreateEmployeeSystemUserResponse
                        {
                            Id = systemUser.Id,
                            Email = systemUser.Email,
                            Created = systemUser.Created,
                            Role = EIdentityRole.Employee
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
}