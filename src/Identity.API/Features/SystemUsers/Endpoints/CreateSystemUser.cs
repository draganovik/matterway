using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using Identity.API.Features.Shared;
using Identity.API.Features.SystemUsers.Contracts;
using Identity.API.Features.SystemUsers.Data;
using Identity.API.Features.SystemUsers.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Identity.API.Features.SystemUsers.Endpoints;

public class CreateSystemUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("SystemUsers", Handler)
            .WithName("CreateSystemUser").WithSummary("Create a new system user.")
            .WithTags("SystemUsers")
            .Produces<SystemUserBaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<SystemUserBaseResponse>, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(
            SystemUserBaseRequest request,
            HttpContext httpContext,
            ISystemUserRepository systemUserRepository,
            IMapper mapper,
            IPasswordHasher<SystemUser> passwordHasher)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        var roleClaim = identity?.FindFirst(ClaimTypes.Role)?.Value;
        var hasRole = Enum.TryParse(roleClaim, out SystemUserRole currentUserRole);

        if (!hasRole)
        {
            request.Role ??= SystemUserRole.Customer;
            if (request.Role != SystemUserRole.Customer)
            {
                return TypedResults.Forbid();
            }
        }
        else
        {
            if (currentUserRole == SystemUserRole.Customer)
            {
                return TypedResults.BadRequest(CreateProblemDetails("Customer can not create other users"));
            }

            if (currentUserRole == SystemUserRole.Manager && request.Role == SystemUserRole.Admin)
            {
                return TypedResults.BadRequest(CreateProblemDetails("Manager can not create Admin"));
            }

            if (currentUserRole == SystemUserRole.Manager && request.Role == SystemUserRole.Manager)
            {
                return TypedResults.BadRequest(CreateProblemDetails("Manager can not create Manager"));
            }
        }

        var systemUser = mapper.Map<SystemUser>(request);
        systemUser.Role = request.Role ?? SystemUserRole.Customer;
        systemUser.PasswordHash = passwordHasher.HashPassword(systemUser, request.Password!);

        var createdSystemUser = await systemUserRepository.Create(systemUser);
        if (createdSystemUser is null)
        {
            return TypedResults.BadRequest(CreateProblemDetails("User is not created"));
        }

        var location = ResourceUrlHelper.BuildResourceLocation(httpContext, $"SystemUsers/{createdSystemUser.Id}");

        return TypedResults.Created(location, mapper.Map<SystemUserBaseResponse>(createdSystemUser));
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