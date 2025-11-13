using Matterway.ServiceDefaults;

namespace Matterway.Ordering.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection ConfigureCors(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder.Configuration);

        var configuredOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                                ?? ["http://localhost:3000", "http://localhost:3001"];

        var storefrontOrigin = builder.Configuration.ResolveServiceUri("storefront-web")
            .GetLeftPart(UriPartial.Authority);

        var allowedOrigins = configuredOrigins.Append(storefrontOrigin)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy => policy
                .WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });

        return builder.Services;
    }
}