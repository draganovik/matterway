using Asp.Versioning;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Identity.Api.Features.SystemUsers.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Identity.Api.Features.SystemUsers.Endpoints;

public class DeleteSystemUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("SystemUsers/{id:guid}", Handler)
            .WithName("DeleteSystemUser").WithSummary("Delete system user by id.")
            .WithTags("SystemUsers")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound>> Handler(
        Guid id,
        ISystemUserRepository systemUserRepository)
    {
        var isDeleted = await systemUserRepository.Delete(id);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}