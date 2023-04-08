using AutoMapper;
using Identity.API.Data;
using Identity.API.Entities;
using Identity.API.Helpers;
using Identity.API.Models.SessionModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
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

    public static async Task<Results<Ok<IEnumerable<SessionBaseResponseModel>>, NoContent>> QuerySessions(IdentityDbContext db, IMapper mapper)
    {
        var ss = await db.Session.Include(s => s.SystemUser).ToListAsync();
        return ss is IEnumerable<Session> value && value.Any()
            ? TypedResults.Ok(mapper.Map<IEnumerable<SessionBaseResponseModel>>(value))
            : TypedResults.NoContent();
    }

    [Authorize]
    public static async Task<Results<Ok<SessionBaseResponseModel>, UnauthorizedHttpResult>> IntrospectSession(HttpContext context, IdentityDbContext db, IMapper mapper)
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
        var currentSession = await db.Session
            .Where(model => model.SystemUserId == systemUserId && model.Token == token).FirstOrDefaultAsync();
        return TypedResults.Ok(mapper.Map<SessionBaseResponseModel>(currentSession));
    }

    public static async Task<Results<Ok<SessionBaseResponseModel>, BadRequest<string>>> CreateSession(SessionBaseRequestModel request, IdentityDbContext db, IConfiguration configuration, IPasswordHasher<SystemUser> passwordHasher, IMapper mapper)
    {
        var existingUser = await db.SystemUser
            .SingleOrDefaultAsync(u => u.Email == request.Email);
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
        await db.Session.AddAsync(session);
        await db.SaveChangesAsync();
        return TypedResults.Ok(mapper.Map<SessionBaseResponseModel>(session));
    }

    public static async Task<Results<Ok<SessionBaseResponseModel>, BadRequest<object>, UnauthorizedHttpResult>> RefreshSession(SessionRefreshBaseRequestModel request, HttpContext context, IdentityDbContext db, IConfiguration configuration, IMapper mapper)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid systemUserId))
        {
            return TypedResults.Unauthorized();
        }
        var session = await db.Session
            .Include(s => s.SystemUser)
            .SingleOrDefaultAsync(s => s.SystemUserId == systemUserId && s.RefreshToken == request.RefreshToken && s.Expires > DateTime.UtcNow);
        if (session == null)
        {
            return TypedResults.BadRequest<object>(new { message = "Token did not expire or refresh token was invalid." });
        }
        if (session.IsExpiredRefresh())
        {
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
        db.Session.Update(session);
        await db.SaveChangesAsync();
        return TypedResults.Ok(mapper.Map<SessionBaseResponseModel>(session));
    }

    [Authorize]
    public static async Task<Results<Ok, NotFound, UnauthorizedHttpResult>> RevokeSession(HttpContext context, IdentityDbContext db)
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
        var affected = await db.Session
            .Where(model => model.SystemUserId == systemUserId && model.Token == token)
            .ExecuteDeleteAsync();
        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }
}
