using Asp.Versioning;
using AutoMapper;
using Identity.API.Features.SystemUsers.Contracts;
using Identity.API.Features.SystemUsers.Data;
using Identity.API.Features.SystemUsers.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Identity.API.Features.SystemUsers.Endpoints;

public class UpdateSystemUserById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("SystemUsers/{id:guid}", Handler)
            .WithName("UpdateSystemUserById").WithSummary("Update system user by id.")
            .WithTags("SystemUsers")
            .Produces<SystemUserBaseResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<SystemUserBaseResponse>, NotFound>> Handler(
        Guid id,
        SystemUserBaseRequest request,
        ISystemUserRepository systemUserRepository,
        IMapper mapper)
    {
        var updatedUser = await systemUserRepository.Update(id, request);
        return updatedUser is not null
            ? TypedResults.Ok(mapper.Map<SystemUserBaseResponse>(updatedUser))
            : TypedResults.NotFound();
    }
}