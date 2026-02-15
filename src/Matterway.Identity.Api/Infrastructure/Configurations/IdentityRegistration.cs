using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Infrastructure.Configurations;

public static class IdentityRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureIdentity()
        {
            builder.Services.AddIdentityCore<SystemUser>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<IdentityDbComposer>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            return builder;
        }
    }
}