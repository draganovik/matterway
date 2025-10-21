using Identity.API.Features.Sessions.Data;
using Identity.API.Features.SystemUsers.Data;
using Identity.API.Features.SystemUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Extensions;

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