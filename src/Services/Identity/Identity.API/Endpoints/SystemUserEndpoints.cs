using AutoMapper;
using Identity.API.Entities;
using Identity.API.Models.SystemUserModels;
using Identity.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Identity.API.Endpoints;

public static class SystemUserEndpoints
{
    public static void MapSystemUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/SystemUsers").WithTags(nameof(SystemUser));

        group.MapGet("/", QuerySystemUsers)
            .WithName("QuerySystemUsers").WithOpenApi(operation => new(operation)
            {
                Summary = "Query System Users",
            });

        group.MapGet("/{id}", GetSystemUserById)
            .WithName("GetSystemUserById").WithOpenApi(operation => new(operation)
            {
                Summary = "Get System User By Id",
            });

        group.MapPatch("/{id}", UpdateSystemUserById)
            .WithName("UpdateSystemUserById").WithOpenApi(operation => new(operation)
            {
                Summary = "Update System User By Id",
            });

        group.MapPost("/", CreateSystemUser)
            .WithName("CreateSystemUser").WithOpenApi(operation => new(operation)
            {
                Summary = "Create System User",
            });

        group.MapDelete("/{id}", DeleteSystemUser)
            .WithName("DeleteSystemUser").WithOpenApi(operation => new(operation)
            {
                Summary = "Delete System User",
            });
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<SystemUserBaseResponseModel>>, NoContent>> QuerySystemUsers([FromQuery] int pageIndex, [FromQuery] int pageSize, ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        return await systemUserRepository.Query(pageIndex, pageSize)
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
    public static async Task<Results<Ok<SystemUserBaseResponseModel>, NotFound, BadRequest<object>>> UpdateSystemUserById(Guid id, SystemUserBaseRequestModel requestModel, ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var updatedUser = await systemUserRepository.Update(id, requestModel);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<SystemUserBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    public static async Task<Results<Created<SystemUserBaseResponseModel>, BadRequest<object>, ForbidHttpResult>> CreateSystemUser(SystemUserBaseRequestModel requestModel, HttpContext httpContext, ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var identity = httpContext.User.Identity as ClaimsIdentity;

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
        {
            requestModel.Role ??= SystemUserRole.Customer;
            if (requestModel.Role != SystemUserRole.Customer)
            {
                return TypedResults.Forbid();
            }
        }
        if (userRole == SystemUserRole.Manager && requestModel.Role == SystemUserRole.Admin)
        {
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors = new List<string> { "Manager can not create Admin" } });
        }

        if (userRole == SystemUserRole.Manager && requestModel.Role == SystemUserRole.Manager)
        {
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors = new List<string> { "Manager can not create Manager" } });
        }

        if (userRole == SystemUserRole.Customer)
        {
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors = new List<string> { "Customer can not create other users" } });
        }

        var systemUserModel = mapper.Map<SystemUser>(requestModel);
        systemUserModel.PasswordHash = new PasswordHasher<SystemUser>().HashPassword(systemUserModel, requestModel.Password!);
        var createdSystemUser = await systemUserRepository.Create(systemUserModel);
        if (createdSystemUser is null)
        {
            return TypedResults.BadRequest<object>(new { message = "User is not created" });
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
