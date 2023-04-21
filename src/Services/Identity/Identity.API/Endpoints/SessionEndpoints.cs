using AutoMapper;
using Identity.API.Entities;
using Identity.API.Helpers;
using Identity.API.Models.SessionModels;
using Identity.API.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using SharedProject.ModelTemplates;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Identity.API.Endpoints;

public static class SessionEndpoints
{
    public static void MapSessionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Sessions").WithTags(nameof(Session));

        group.MapGet("/", QuerySessions)
            .WithName("QuerySessions").WithOpenApi(operation => new(operation)
            {
                Summary = "Query Sessions",
            });

        group.MapGet("/introspect", IntrospectSession)
            .WithName("IntrospectSession").WithOpenApi(operation => new(operation)
            {
                Summary = "Introspect Session",
            });

        group.MapPost("/create", CreateSession)
            .WithName("CreateSession").WithOpenApi(operation => new(operation)
            {
                Summary = "Create Session",
            });

        group.MapPost("/refresh", RefreshSession)
            .WithName("RefreshSession").WithOpenApi(operation => new(operation)
            {
                Summary = "Refresh Session",
            });

        group.MapDelete("/revoke", RevokeSession)
            .WithName("RevokeSession").WithOpenApi(operation => new(operation)
            {
                Summary = "Revoke Session",
            });
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<PaginationResponse<SessionBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>> QuerySessions([FromQuery] int page, [FromQuery] int pageSize, HttpContext httpContext, ISessionRepository sessionRepository, IMapper mapper)
    {
        if (page < 1 || pageSize < 1)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid page or pageSize.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Page and pageSize must be greater than zero.",
            };
            var results = new List<ValidationResult>();
            if (page < 1)
            {
                results.Add(new ValidationResult("Page must be greater than zero.", new[] { nameof(page) }));
            }
            if (pageSize < 1)
            {
                results.Add(new ValidationResult("PageSize must be greater than zero.", new[] { nameof(pageSize) }));
            }

            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var total = await sessionRepository.GetTotalEntities();
        var entities = await sessionRepository.Query(page, pageSize);
        var baseUri = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/Sessions");

        var paginationResponse = new PaginationResponse<SessionBaseResponseModel>(total, page, pageSize, mapper.Map<IEnumerable<SessionBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<Session> value && value.Any()
                ? TypedResults.Ok(paginationResponse)
                : TypedResults.NoContent();
    }

    [Authorize]
    public static async Task<Results<Ok<SessionBaseResponseModel>, UnauthorizedHttpResult>> IntrospectSession(HttpContext context, ISessionRepository sessionRepository, IMapper mapper)
    {
        var user = context.User;
        var identity = user.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid systemUserId))
        {
            return TypedResults.Unauthorized();
        }
        // Get the token from the context
        var token = context.GetTokenAsync("access_token").Result;
        if (token == null)
        {
            return TypedResults.Unauthorized();
        }

        var currentSession = await sessionRepository.GetByToken(token);
        return TypedResults.Ok(mapper.Map<SessionBaseResponseModel>(currentSession));
    }

    public static async Task<Results<Ok<SessionBaseResponseModel>, BadRequest<ProblemDetails>>> CreateSession(SessionBaseRequestModel requestModel, ISessionRepository sessionRepository, ISystemUserRepository systemUserRepository, IConfiguration configuration, IPasswordHasher<SystemUser> passwordHasher, IMapper mapper)
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

        var existingUser = await systemUserRepository.GetByEmail(requestModel.Email!);
        if (existingUser == null || passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash!, requestModel.Password!) != PasswordVerificationResult.Success)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }
        // Generate a JWT for the user session
        var (token, tdescriptor) = JwtOperations.Generate(existingUser, configuration);
        var (refresh, rdescriptor) = JwtOperations.Generate(existingUser, configuration, true);
        // Create a new session for the user
        var session = new Session
        {
            SystemUserId = existingUser.Id,
            Token = token,
            RefreshToken = refresh,
            Created = tdescriptor.IssuedAt ?? DateTime.Now,
            Expires = tdescriptor.Expires ?? DateTime.Now,
            RefreshExpires = rdescriptor.Expires ?? DateTime.Now
        };
        var createdSession = await sessionRepository.Create(session);
        return TypedResults.Ok(mapper.Map<SessionBaseResponseModel>(createdSession));
    }

    public static async Task<Results<Ok<SessionBaseResponseModel>, BadRequest<ProblemDetails>, UnauthorizedHttpResult>> RefreshSession(SessionRefreshBaseRequestModel requestModel, ISessionRepository sessionRepository, IConfiguration configuration, IMapper mapper)
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

        var session = await sessionRepository.GetByRefreshToken(requestModel.RefreshToken!);
        if (session == null || session.Expires > DateTime.Now)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Token did not expire or refresh token was invalid."
            };
            return TypedResults.BadRequest(problemDetails);
        }

        if (session.IsExpiredRefresh())
        {
            await sessionRepository.DeleteByRefreshToken(requestModel.RefreshToken!);
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Refresh token has expired."
            };
            return TypedResults.BadRequest(problemDetails);
        }
        // Generate a new JWT for the user session
        var (token, tdescriptor) = JwtOperations.Generate(session.SystemUser!, configuration);
        var (refresh, rdescriptor) = JwtOperations.Generate(session.SystemUser!, configuration, true);
        // Update the session with the new tokens
        session.Token = token;
        session.RefreshToken = refresh;
        session.Created = tdescriptor.IssuedAt ?? DateTime.Now;
        session.Expires = tdescriptor.Expires ?? DateTime.Now;
        session.RefreshExpires = rdescriptor.Expires ?? DateTime.Now;
        var refreshedSession = await sessionRepository.Refresh(session);
        return TypedResults.Ok(mapper.Map<SessionBaseResponseModel>(refreshedSession));
    }

    [Authorize]
    public static async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> RevokeSession(HttpContext context, ISessionRepository sessionRepository)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out _))
        {
            return TypedResults.Unauthorized();
        }
        // Get the token from the context
        var token = context.GetTokenAsync("access_token").Result;
        var isDeleted = await sessionRepository.DeleteByToken(token!);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
