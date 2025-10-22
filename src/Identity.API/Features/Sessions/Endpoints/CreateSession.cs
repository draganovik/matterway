using Asp.Versioning;
using AutoMapper;
using Identity.API.Features.Sessions.Contracts;
using Identity.API.Features.Sessions.Data;
using Identity.API.Features.Sessions.Domain;
using Identity.API.Features.Sessions.Services;
using Identity.API.Features.SystemUsers.Data;
using Identity.API.Features.SystemUsers.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Common.Infrastructure.Interfaces;

namespace Identity.API.Features.Sessions.Endpoints;

public class CreateSession : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Sessions/create", Handler)
            .WithName("CreateSession").WithSummary("Create a new session.")
            .WithTags("Sessions")
            .Produces<SessionBaseResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<SessionBaseResponse>, BadRequest<ProblemDetails>>> Handler(
        SessionBaseRequest request,
        ISessionRepository sessionRepository,
        ISystemUserRepository systemUserRepository,
        IConfiguration configuration,
        IPasswordHasher<SystemUser> passwordHasher,
        IMapper mapper)
    {
        var existingUser = await systemUserRepository.GetByEmail(request.Email!);
        if (existingUser is null ||
            passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash!, request.Password!) !=
            PasswordVerificationResult.Success)
        {
            return TypedResults.BadRequest(CreateProblemDetails("Cannot create entity"));
        }

        var (token, tokenDescriptor) = JwtOperations.Generate(existingUser, configuration);
        var (refreshToken, refreshDescriptor) = JwtOperations.Generate(existingUser, configuration, true);

        var session = new Session
        {
            SystemUserId = existingUser.Id,
            Token = token,
            RefreshToken = refreshToken,
            Created = tokenDescriptor.IssuedAt ?? DateTime.Now,
            Expires = tokenDescriptor.Expires ?? DateTime.Now,
            RefreshExpires = refreshDescriptor.Expires ?? DateTime.Now
        };

        var createdSession = await sessionRepository.Create(session);
        return TypedResults.Ok(mapper.Map<SessionBaseResponse>(createdSession));
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