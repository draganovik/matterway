using System.Security.Claims;
using System.Text;
using Matterway.Common.Services.Brokers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Matterway.Catalog.Api.Extensions;

public static class AuthRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureAuthentication()
        {
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
                    var signingKey = builder.Configuration["Jwt:Key"]
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
                            if (!Guid.TryParse(identifier, out _) ||
                                context.SecurityToken is not JsonWebToken jwtToken ||
                                string.IsNullOrWhiteSpace(jwtToken.EncodedToken))
                            {
                                context.Fail("Unauthorized");
                                return;
                            }

                            var broker =
                                context.HttpContext.RequestServices.GetRequiredService<IIdentityServiceBroker>();
                            if (await broker.ValidateTokenAsync(jwtToken.EncodedToken) is null)
                                context.Fail("Unauthorized");
                        }
                    };
                });
            builder.Services.AddAuthorization();

            return builder;
        }
    }
}