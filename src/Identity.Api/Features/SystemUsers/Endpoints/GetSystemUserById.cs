using Asp.Versioning;
using AutoMapper;
using Identity.Api.Features.SystemUsers.Contracts;
using Identity.Api.Features.SystemUsers.Data;
using Identity.Api.Features.SystemUsers.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Identity.Api.Features.SystemUsers.Endpoints;

public class GetSystemUserById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("SystemUsers/{id:guid}", Handler)
            .WithName("GetSystemUserById").WithSummary("Get system user by id.")
            .WithTags("SystemUsers")
            .Produces<SystemUserBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<SystemUserBaseResponse>, NotFound>> Handler(
        Guid id,
        ISystemUserRepository systemUserRepository,
        IMapper mapper)
    {
        return await systemUserRepository.GetById(id)
            is SystemUser value
            ? TypedResults.Ok(mapper.Map<SystemUserBaseResponse>(value))
            : TypedResults.NotFound();
    }
}