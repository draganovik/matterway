using AutoMapper;
using Identity.API.Entities;
using Identity.API.Helpers;
using Identity.API.Models.SessionModels;
using Identity.API.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Shared.Enums;
using System.Security.Claims;

namespace Identity.API.Endpoints;

public static class SessionEndpoints
{
    public static void MapSessionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Sessions").WithTags(nameof(Session));

        group.MapGet("/", QuerySessions)
            .WithName("GetAllSessions").WithOpenApi();

        group.MapGet("/introspect", IntrospectSession)
            .WithName("IntrospectSession").WithOpenApi();

        group.MapPost("/create", CreateSession)
            .WithName("CreateSession").WithOpenApi();

        group.MapPost("/refresh", RefreshSession)
            .WithName("RefreshSession").WithOpenApi();

        group.MapDelete("/revoke", RevokeSession)
            .WithName("RevokeSession").WithOpenApi();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<SessionBaseResponseModel>>, NoContent>> QuerySessions(ISessionRepository sessionRepository, IMapper mapper)
    {
        var ss = await sessionRepository.Query();
        return ss is IEnumerable<Session> value && value.Any()
            ? TypedResults.Ok(mapper.Map<IEnumerable<SessionBaseResponseModel>>(value))
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
        if (token == null) return TypedResults.Unauthorized();
        var currentSession = await sessionRepository.GetByToken(token);
        return TypedResults.Ok(mapper.Map<SessionBaseResponseModel>(currentSession));
    }

    public static async Task<Results<Ok<SessionBaseResponseModel>, BadRequest<string>>> CreateSession(SessionBaseRequestModel request, ISessionRepository sessionRepository, ISystemUserRepository systemUserRepository, IConfiguration configuration, IPasswordHasher<SystemUser> passwordHasher, IMapper mapper)
    {
        var existingUser = await systemUserRepository.GetByEmail(request.Email);
        if (existingUser == null || passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash!, request.Password) != PasswordVerificationResult.Success)
        {
            return TypedResults.BadRequest("Invalid email or password.");
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
            Created = tdescriptor.IssuedAt ?? DateTime.UtcNow,
            Expires = tdescriptor.Expires ?? DateTime.UtcNow,
            RefreshExpires = rdescriptor.Expires ?? DateTime.UtcNow
        };
        var createdSession = await sessionRepository.Create(session);
        return TypedResults.Ok(mapper.Map<SessionBaseResponseModel>(createdSession));
    }

    public static async Task<Results<Ok<SessionBaseResponseModel>, BadRequest<object>, UnauthorizedHttpResult>> RefreshSession(SessionRefreshBaseRequestModel request, HttpContext context, ISessionRepository sessionRepository, IConfiguration configuration, IMapper mapper)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid systemUserId))
        {
            return TypedResults.Unauthorized();
        }
        var session = await sessionRepository.GetByRefreshToken(request.RefreshToken);
        if (session == null || session.Expires < DateTime.Now)
        {
            return TypedResults.BadRequest<object>(new { message = "Token did not expire or refresh token was invalid." });
        }
        if (session.SystemUserId != systemUserId)
        {
            return TypedResults.Unauthorized();
        }
        if (session.IsExpiredRefresh())
        {
            await sessionRepository.DeleteByRefreshToken(request.RefreshToken);
            return TypedResults.BadRequest<object>(new { message = "Refresh token has expired." });
        }
        // Generate a new JWT for the user session
        var (token, tdescriptor) = JwtOperations.Generate(session.SystemUser!, configuration);
        var (refresh, rdescriptor) = JwtOperations.Generate(session.SystemUser!, configuration, true);
        // Update the session with the new tokens
        session.Token = token;
        session.RefreshToken = refresh;
        session.Created = tdescriptor.IssuedAt ?? DateTime.UtcNow;
        session.Expires = tdescriptor.Expires ?? DateTime.UtcNow;
        session.RefreshExpires = rdescriptor.Expires ?? DateTime.UtcNow;
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
