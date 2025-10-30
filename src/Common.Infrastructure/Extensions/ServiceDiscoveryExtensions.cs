using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Common.Infrastructure.Extensions;

public static class ServiceDiscoveryExtensions
{
    public static Uri ResolveServiceUri(this IConfiguration configuration, string aspireServiceName,
        string? configurationKey = null)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (string.IsNullOrWhiteSpace(aspireServiceName))
        {
            throw new ArgumentException("Service name must be provided.", nameof(aspireServiceName));
        }

        if (configurationKey is not null)
        {
            var explicitValue = configuration[configurationKey];
            if (!string.IsNullOrWhiteSpace(explicitValue))
            {
                return CreateUri(explicitValue, configurationKey);
            }
        }

        var inferredKey = configurationKey ?? $"Services:{ToPascalCase(aspireServiceName)}:Url";
        var configuredValue = configuration[inferredKey];
        if (!string.IsNullOrWhiteSpace(configuredValue))
        {
            return CreateUri(configuredValue, inferredKey);
        }

        return new Uri($"http://{aspireServiceName}", UriKind.Absolute);
    }

    private static Uri CreateUri(string value, string key)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException(
                $"Configuration value '{value}' for '{key}' is not a valid absolute URI.");
        }

        return uri;
    }

    private static string ToPascalCase(string value)
    {
        var parts = value.Split(['-', '_'], StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Select(static part =>
        {
            if (part.Length == 0)
            {
                return string.Empty;
            }

            if (part.Length == 1)
            {
                return part.ToUpperInvariant();
            }

            return char.ToUpperInvariant(part[0]) + part[1..].ToLowerInvariant();
        }));
    }
}