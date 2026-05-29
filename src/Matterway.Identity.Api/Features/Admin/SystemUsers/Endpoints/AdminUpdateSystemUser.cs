using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Features.Admin.SystemUsers.Contracts;
using Matterway.Identity.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Identity.Api.Features.Admin.SystemUsers.Endpoints;

public class AdminUpdateSystemUser : IEndpoint
{
    private const string RouteName = nameof(AdminUpdateSystemUser);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "system-users/{id:guid}", Handler)
            .WithName(RouteName).WithSummary("[admin] Update system user by id")
            .WithTags("SystemUsers")
            .Produces<AdminUpdateSystemUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminUpdateSystemUserResponse>, BadRequest<ProblemDetails>, NotFound>> Handler(
        Guid id,
        AdminUpdateSystemUserRequest request,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        IdentityDbComposer identityDb,
        UserManager<SystemUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        var strategy = identityDb.Database.CreateExecutionStrategy();
        return await ExecutionStrategyExtensions
            .ExecuteAsync<Results<Ok<AdminUpdateSystemUserResponse>, BadRequest<ProblemDetails>, NotFound>>(
                strategy,
                async () =>
                {
                    await using var transaction = await identityDb.Database.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        var user = await userManager.FindByIdAsync(id.ToString());
                        if (user is null) return TypedResults.NotFound();

                        var userRole = await IdentityRoleAdapter.GetPrimaryRoleAsync(userManager, user);
                        var isManager = RequestIdentity.AsManager(httpContext.User);

                        if (!isManager && userRole != EIdentityRole.Customer)
                            return TypedResults.BadRequest(
                                CreateProblemDetails("Operators can only update Customer users"));

                        if (!string.IsNullOrWhiteSpace(request.Email))
                        {
                            user.Email = request.Email;
                            user.NormalizedEmail = userManager.NormalizeEmail(request.Email);
                            user.UserName = request.Email;
                            user.NormalizedUserName = userManager.NormalizeName(request.Email);
                        }

                        var updateResult = await userManager.UpdateAsync(user);
                        if (!updateResult.Succeeded)
                            return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                                updateResult.Errors.Select(error => error.Description))));

                        if (!string.IsNullOrWhiteSpace(request.Password))
                        {
                            var resetResult = await userManager.ResetPasswordAsync(user,
                                await userManager.GeneratePasswordResetTokenAsync(user), request.Password);
                            if (!resetResult.Succeeded)
                                return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                                    resetResult.Errors.Select(error => error.Description))));
                        }

                        await transaction.CommitAsync(cancellationToken);

                        return TypedResults.Ok(new AdminUpdateSystemUserResponse
                        {
                            Id = user.Id,
                            Email = user.Email,
                            Created = user.Created,
                            Role = userRole
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