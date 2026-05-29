using Matterway.Identity.Api.Features.Admin.SystemUsers.Contracts;
using Matterway.Identity.Api.Infrastructure.Persistence.SystemUserEntity;

namespace Matterway.Identity.Api.Features.Admin.SystemUsers.Endpoints;

public class AdminGetSystemUserById : IEndpoint
{
    private const string RouteName = nameof(AdminGetSystemUserById);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "system-users/{id:guid}", Handler)
            .WithName(RouteName).WithSummary("[admin] Get system user by id")
            .WithTags("SystemUsers")
            .Produces<AdminGetSystemUserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminGetSystemUserResponse>, NotFound>> Handler(
        Guid id,
        ISystemUserRepository systemUserRepository,
        CancellationToken cancellationToken)
    {
        var user = await systemUserRepository.GetById(id, cancellationToken);
        if (user is null) return TypedResults.NotFound();

        return TypedResults.Ok(new AdminGetSystemUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Created = user.Created,
            Role = user.Role
        });
    }
}