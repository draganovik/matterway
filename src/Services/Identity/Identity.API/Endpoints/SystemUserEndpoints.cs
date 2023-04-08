using AutoMapper;
using Identity.API.Data;
using Identity.API.Entities;
using Identity.API.Enums;
using Identity.API.Models.SystemUserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
    public static async Task<Results<Ok<IEnumerable<SystemUserBaseResponseModel>>, NotFound>> QuerySystemUsers(IdentityDbContext db, IMapper mapper)
    {
        return await db.SystemUser.AsNoTracking()
            .ToListAsync()
            is IEnumerable<SystemUser> value && value.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<SystemUserBaseResponseModel>>(value))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<SystemUserBaseResponseModel>, NotFound>> GetSystemUserById(Guid id, IdentityDbContext db, IMapper mapper)
    {
        return await db.SystemUser.AsNoTracking()
            .FirstOrDefaultAsync(model => model.Id == id)
            is SystemUser value
                ? TypedResults.Ok(mapper.Map<SystemUserBaseResponseModel>(value))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<SystemUserBaseResponseModel>, NotFound>> UpdateSystemUserById(Guid id, SystemUserBaseRequestModel requestModel, IdentityDbContext db, IPasswordHasher<SystemUser> passwordHasher, IMapper mapper)
    {
        var currentUserModel = await db.SystemUser.FindAsync(id);
        if (currentUserModel is null)
        {
            return TypedResults.NotFound();
        }
        var affected = await db.SystemUser
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Email, requestModel.Email)
                  .SetProperty(m => m.Created, DateTime.UtcNow)
                  .SetProperty(m => m.Role, requestModel.Role)
                  .SetProperty(m => m.PasswordHash, passwordHasher.HashPassword(currentUserModel, requestModel.Password!))
                );
        var updatedUser = await db.SystemUser.FindAsync(id);
        return affected == 1 ? TypedResults.Ok(mapper.Map<SystemUserBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<SystemUserBaseResponseModel>, BadRequest>> CreateSystemUser(SystemUserBaseRequestModel requestModel, IdentityDbContext db, IMapper mapper)
    {
        var systemUserModel = mapper.Map<SystemUser>(requestModel);
        db.SystemUser.Add(systemUserModel);
        var states = await db.SaveChangesAsync();
        if (states == 0)
        {
            return TypedResults.BadRequest();
        }
        return TypedResults.Created($"/api/SystemUserModels/{systemUserModel.Id}", mapper.Map<SystemUserBaseResponseModel>(systemUserModel));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteSystemUser(Guid id, IdentityDbContext db, IMapper mapper)
    {
        var affected = await db.SystemUser
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();

        return affected == 1 ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
