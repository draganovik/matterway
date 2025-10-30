using Matterway.Identity.Api.Features.Sessions.Data;
using Matterway.Identity.Api.Features.SystemUsers.Data;
using Matterway.Identity.Api.Features.SystemUsers.Domain;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Extensions;

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