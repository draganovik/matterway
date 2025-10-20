using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.ServiceBrokers;

namespace Catalog.API.Configurations;

public static class AuthenticationConfiguration
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                var signingKey = configuration["Jwt:Key"]
                                 ?? throw new InvalidOperationException("JWT signing key not configured.");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var identifier = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        if (!Guid.TryParse(identifier, out _) || context.SecurityToken is not JwtSecurityToken jwt ||
                            string.IsNullOrWhiteSpace(jwt.RawData))
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var broker = context.HttpContext.RequestServices.GetRequiredService<IIdentityServiceBroker>();
                        if (await broker.ValidateTokenAsync(jwt.RawData) is null)
                        {
                            context.Fail("Unauthorized");
                        }
                    }
                };
            });
        services.AddAuthorization();
        return services;
    }
}