using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Matterway.ServiceDefaults.Extensions;

public static class AuthenticationExtensions
{
    public static IHostApplicationBuilder ConfigureAuthentication(
        this IHostApplicationBuilder builder,
        Action<JwtBearerOptions>? configure = null)
    {
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];
        if (string.IsNullOrWhiteSpace(issuer))
            throw new InvalidOperationException("JWT issuer not configured.");
        if (string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("JWT audience not configured.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var signingKey = builder.Configuration["Jwt:Key"]
                                 ?? throw new InvalidOperationException("JWT signing key not configured.");

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                };
                configure?.Invoke(options);
            });

        builder.Services.AddAuthorization();
        return builder;
    }
}