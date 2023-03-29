using AutoMapper;
using Identity.API.Data;
using Identity.API.Entities;
using Identity.API.Helpers;
using Identity.API.Models;
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

        var group = routes.MapGroup("/api/Session").WithTags(nameof(Session));

        group.MapPost("/create", async Task<Results<Ok<SessionPostResponse>, BadRequest<string>>>
            (SessionPostRequest request, IdentityDbContext db, IConfiguration configuration, IPasswordHasher<SystemUser> passwordHasher, IMapper mapper) =>
        {
            var existingUser = await db.SystemUser
                .SingleOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser == null || passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, request.Password) != PasswordVerificationResult.Success)
            {

                return TypedResults.BadRequest("Invalid email or password.");
            }

            // Generate a JWT for the user session
            var (token, tdescriptor) = JwtOperations.Generate(existingUser, configuration);
            var (refresh, _) = JwtOperations.Generate(existingUser, configuration, true);

            // Create a new session for the user
            var session = new Session
            {
                SystemUserId = existingUser.Id,
                Token = token,
                RefreshToken = refresh,
                Created = tdescriptor.IssuedAt ?? DateTime.UtcNow,
                Expires = tdescriptor.Expires ?? DateTime.UtcNow,
            };
            await db.Session.AddAsync(session);
            await db.SaveChangesAsync();

            return TypedResults.Ok(mapper.Map<SessionPostResponse>(session));
        }).WithName("CreateSession")
        .WithOpenApi();

        group.MapGet("/", async Task<Results<Ok<IEnumerable<SessionPostResponse>>, NoContent>> (IdentityDbContext db, IMapper mapper) =>
        {
            var ss = await db.Session.Include(s => s.SystemUser).ToListAsync();
            return ss is IEnumerable<Session> value && value.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<SessionPostResponse>>(value))
                : TypedResults.NoContent();
        })
        .WithName("GetAllSessions")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Session>, NotFound>> (Guid id, IdentityDbContext db) =>
        {
            return await db.Session.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Session model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetSessionById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, Session session, IdentityDbContext db) =>
        {
            var affected = await db.Session
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, session.Id)
                  .SetProperty(m => m.SystemUserId, session.SystemUserId)
                  .SetProperty(m => m.Token, session.Token)
                  .SetProperty(m => m.RefreshToken, session.RefreshToken)
                  .SetProperty(m => m.Created, session.Created)
                  .SetProperty(m => m.Expires, session.Expires)
                );

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateSession")
        .WithOpenApi();

        group.MapDelete("/revoke", [Authorize] async Task<Results<Ok, NotFound, UnauthorizedHttpResult>> (HttpContext context, IdentityDbContext db) =>
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
        })
        .WithName("RevokeSession")
        .WithOpenApi();
    }
}
