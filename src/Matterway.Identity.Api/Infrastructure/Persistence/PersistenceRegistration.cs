using Matterway.Identity.Api.Infrastructure.Persistence.SystemUserEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Identity.Api.Infrastructure.Persistence;

public static class PersistenceRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigurePersistence()
        {
            var connectionString = builder.Configuration.GetConnectionString("IdentityDb")
                                   ?? throw new InvalidOperationException(
                                       "Connection string 'IdentityDb' not found.");

            builder.Services.AddDbContext<IdentityDbComposer>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));
            builder.Services.AddScoped<ISystemUserRepository, EfPgSystemUserRepository>();

            return builder;
        }
    }
}