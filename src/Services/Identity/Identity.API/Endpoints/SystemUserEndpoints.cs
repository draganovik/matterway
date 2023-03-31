using AutoMapper;
using Identity.API.Data;
using Identity.API.Entities;
using Identity.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace Identity.API.Endpoints;

public static class SystemUserEndpoints
{
    public static void MapSystemUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Account").WithTags(nameof(SystemUser));

        group.MapGet("/", async Task<Results<Ok<IEnumerable<SystemUserGetResponse>>, NotFound>> (IdentityDbContext db, IMapper mapper) =>
        {
            return await db.SystemUser.AsNoTracking()
                .ToListAsync()
                is IEnumerable<SystemUser> value && value.Any()
                    ? TypedResults.Ok(mapper.Map<IEnumerable<SystemUserGetResponse>>(value))
                    : TypedResults.NotFound();
        })
        .WithName("GetAllSystemUsers")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<SystemUser>, NotFound>> (Guid id, IdentityDbContext db) =>
        {
            return await db.SystemUser.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is SystemUser model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetSystemUserById")
        .WithOpenApi();

        group.MapPut("update/{id}", async Task<Results<Ok, NotFound>> (Guid id, SystemUser systemUser, IdentityDbContext db) =>
        {
            var affected = await db.SystemUser
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, systemUser.Id)
                  .SetProperty(m => m.Email, systemUser.Email)
                  .SetProperty(m => m.PasswordHash, systemUser.PasswordHash)
                  .SetProperty(m => m.Created, systemUser.Created)
                  .SetProperty(m => m.Role, systemUser.Role)
                );

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateSystemUser")
        .WithOpenApi();

        group.MapPost("/register", async (SystemUser systemUser, IdentityDbContext db) =>
        {
            db.SystemUser.Add(systemUser);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/SystemUser/{systemUser.Id}", systemUser);
        })
        .WithName("CreateSystemUser")
        .WithOpenApi();

        /*group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, IdentityDbContext db) =>
        {
            var affected = await db.SystemUser
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteSystemUser")
        .WithOpenApi();*/
    }
}
