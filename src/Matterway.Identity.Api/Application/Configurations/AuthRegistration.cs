using System.Security.Claims;
using System.Text;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.Identity.Api.Infrastructure.Services.AuthToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Matterway.Identity.Api.Application.Configurations;

public static class AuthRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureAuthentication()
        {
            builder.Services.Configure<JwtTokenOptions>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddScoped<ITokenService, JwtTokenService>();

            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtTokenOptions>() ?? new JwtTokenOptions();
            if (string.IsNullOrWhiteSpace(jwtOptions.Key))
                throw new InvalidOperationException("JWT signing key not configured.");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = !string.IsNullOrWhiteSpace(jwtOptions.Issuer),
                        ValidateAudience = false,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.NameIdentifier,
                        ClockSkew = TimeSpan.FromMinutes(Math.Max(0, jwtOptions.ClockSkewMinutes))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var tokenUse = context.Principal?.FindFirstValue(JwtTokenService.TokenUseClaim);
                            if (!string.Equals(tokenUse, JwtTokenService.AccessTokenUse, StringComparison.Ordinal))
                            {
                                context.Fail("Unauthorized");
                                return;
                            }

                            var identifier = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                            if (!Guid.TryParse(identifier, out var userId))
                            {
                                context.Fail("Unauthorized");
                                return;
                            }

                            var userManager = context.HttpContext.RequestServices
                                .GetRequiredService<UserManager<SystemUser>>();
                            var user = await userManager.FindByIdAsync(userId.ToString());
                            if (user is null)
                            {
                                context.Fail("Unauthorized");
                                return;
                            }

                            var securityStampClaim =
                                context.Principal?.FindFirstValue(JwtTokenService.SecurityStampClaim);
                            if (string.IsNullOrWhiteSpace(securityStampClaim))
                            {
                                context.Fail("Unauthorized");
                                return;
                            }

                            var securityStamp = await userManager.GetSecurityStampAsync(user);
                            if (!string.Equals(securityStamp, securityStampClaim, StringComparison.Ordinal))
                                context.Fail("Unauthorized");
                        }
                    };
                });

            builder.Services.AddAuthorization();
            return builder;
        }
    }
}