using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AutoMapper;
using Identity.API.Entities;
using Identity.API.Models.SystemUserModels;
using Identity.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Shared.Enums;
using SharedProject.ModelTemplates;

namespace Identity.API.Endpoints;

public static class SystemUserEndpoints
{
    public static void MapSystemUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/SystemUsers").WithTags(nameof(SystemUser));

        group.MapGet("/", QuerySystemUsers)
            .WithName("QuerySystemUsers").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Query System Users"
            });

        group.MapGet("/{id}", GetSystemUserById)
            .WithName("GetSystemUserById").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get System User By Id"
            });

        group.MapPatch("/{id}", UpdateSystemUserById)
            .WithName("UpdateSystemUserById").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Update System User By Id"
            });

        group.MapPost("/", CreateSystemUser)
            .WithName("CreateSystemUser").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Create System User"
            });

        group.MapDelete("/{id}", DeleteSystemUser)
            .WithName("DeleteSystemUser").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Delete System User"
            });
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async
        Task<Results<Ok<PaginationResponse<SystemUserBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>>
        QuerySystemUsers([FromQuery] int page, [FromQuery] int pageSize, HttpContext httpContext,
            ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        if (page < 1 || pageSize < 1)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid page or pageSize.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Page and pageSize must be greater than zero."
            };
            var results = new List<ValidationResult>();
            if (page < 1) results.Add(new ValidationResult("Page must be greater than zero.", new[] { nameof(page) }));
            if (pageSize < 1)
                results.Add(new ValidationResult("PageSize must be greater than zero.", new[] { nameof(pageSize) }));

            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var total = await systemUserRepository.GetTotalEntities();
        var entities = await systemUserRepository.Query(page, pageSize);
        var baseUri =
            new Uri(
                $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/SystemUsers");

        var paginationResponse = new PaginationResponse<SystemUserBaseResponseModel>(total, page, pageSize,
            mapper.Map<IEnumerable<SystemUserBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<SystemUser> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<SystemUserBaseResponseModel>, NotFound>> GetSystemUserById(Guid id,
        ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        return await systemUserRepository.GetById(id)
            is SystemUser value
            ? TypedResults.Ok(mapper.Map<SystemUserBaseResponseModel>(value))
            : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<SystemUserBaseResponseModel>, NotFound, BadRequest<ProblemDetails>>>
        UpdateSystemUserById(Guid id, SystemUserBaseRequestModel requestModel,
            ISystemUserRepository systemUserRepository,
            IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred."
            };
            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var updatedUser = await systemUserRepository.Update(id, requestModel);
        return updatedUser is not null
            ? TypedResults.Ok(mapper.Map<SystemUserBaseResponseModel>(updatedUser))
            : TypedResults.NotFound();
    }

    public static async
        Task<Results<Created<SystemUserBaseResponseModel>, BadRequest<ProblemDetails>, ForbidHttpResult>>
        CreateSystemUser(SystemUserBaseRequestModel requestModel, HttpContext httpContext,
            ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred."
            };
            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var identity = httpContext.User.Identity as ClaimsIdentity;

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
        {
            requestModel.Role ??= SystemUserRole.Customer;
            if (requestModel.Role != SystemUserRole.Customer) return TypedResults.Forbid();
        }

        if (userRole == SystemUserRole.Manager && requestModel.Role == SystemUserRole.Admin)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Manager can not create Admin"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        if (userRole == SystemUserRole.Manager && requestModel.Role == SystemUserRole.Manager)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Manager can not create Manager"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        if (userRole == SystemUserRole.Customer)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Customer can not create other users"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var systemUserModel = mapper.Map<SystemUser>(requestModel);
        systemUserModel.PasswordHash =
            new PasswordHasher<SystemUser>().HashPassword(systemUserModel, requestModel.Password!);
        var createdSystemUser = await systemUserRepository.Create(systemUserModel);
        if (createdSystemUser is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "User is not created"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        return TypedResults.Created($"/api/SystemUsers/{createdSystemUser.Id}",
            mapper.Map<SystemUserBaseResponseModel>(createdSystemUser));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteSystemUser(Guid id,
        ISystemUserRepository systemUserRepository, IMapper mapper)
    {
        var isDeleted = await systemUserRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}