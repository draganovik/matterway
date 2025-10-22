using Identity.Api.Features.Sessions.Data;
using Identity.Api.Features.SystemUsers.Data;
using Identity.Api.Features.SystemUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<SystemUser>, PasswordHasher<SystemUser>>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ISystemUserRepository, SystemUserRepository>();

        return services;
    }
}