using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;

namespace Identity.Api.Extensions;

public static class AuthenticationExtensions
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

                var signingKey = configuration["Jwt:Key"] ??
                                 throw new InvalidOperationException("JWT signing key not configured.");

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

                        if (!Guid.TryParse(identifier, out var userId) ||
                            context.SecurityToken is not JsonWebToken jwtToken ||
                            string.IsNullOrWhiteSpace(jwtToken.EncodedToken))
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var dbContext = context.HttpContext.RequestServices.GetRequiredService<IdentityDbContext>();
                        var sessionExists = await dbContext.Session
                            .AsNoTracking()
                            .AnyAsync(s => s.Token == jwtToken.EncodedToken,
                                context.HttpContext.RequestAborted);

                        if (!sessionExists)
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