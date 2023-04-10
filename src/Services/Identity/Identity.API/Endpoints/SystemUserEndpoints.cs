using AutoMapper;
using Identity.API.Entities;
using Identity.API.Models.SystemUserModels;
using Identity.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Shared.Enums;

namespace Identity.API.Endpoints;

public static class SystemUserEndpoints
{
    public static void MapSystemUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/SystemUsers").WithTags(nameof(SystemUser));

        group.MapGet("/", QuerySystemUsers)
            .WithName("QuerySystemUsers").WithOpenApi();

        group.MapGet("/{id}", GetSystemUserById)
            .WithName("GetSystemUserById").WithOpenApi();

        group.MapPut("/{id}", UpdateSystemUserById)
            .WithName("UpdateSystemUserById").WithOpenApi();

        group.MapPost("/", CreateSystemUser)
            .WithName("CreateSystemUser").WithOpenApi();

        group.MapDelete("/{id}", DeleteSystemUser)
            .WithName("DeleteSystemUser").WithOpenApi();
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<SystemUserBaseResponseModel>>, NoContent>> QuerySystemUsers(ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        return await systemUserRepository.Query()
            is IEnumerable<SystemUser> value && value.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<SystemUserBaseResponseModel>>(value))
                : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<SystemUserBaseResponseModel>, NotFound>> GetSystemUserById(Guid id, ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        return await systemUserRepository.GetById(id)
            is SystemUser value
                ? TypedResults.Ok(mapper.Map<SystemUserBaseResponseModel>(value))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<SystemUserBaseResponseModel>, NotFound>> UpdateSystemUserById(Guid id, SystemUserBaseRequestModel requestModel, ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        var updatedUser = await systemUserRepository.Update(id, requestModel);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<SystemUserBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<SystemUserBaseResponseModel>, BadRequest>> CreateSystemUser(SystemUserBaseRequestModel requestModel, ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        var systemUserModel = mapper.Map<SystemUser>(requestModel);
        systemUserModel.PasswordHash = new PasswordHasher<SystemUser>().HashPassword(systemUserModel, requestModel.Password);
        var createdSystemUser = await systemUserRepository.Create(systemUserModel);
        if (createdSystemUser is null)
        {
            return TypedResults.BadRequest();
        }
        return TypedResults.Created($"/api/SystemUsers/{createdSystemUser.Id}", mapper.Map<SystemUserBaseResponseModel>(createdSystemUser));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteSystemUser(Guid id, ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        var isDeleted = await systemUserRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
