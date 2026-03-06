using Microsoft.Extensions.Configuration;

namespace Matterway.ServiceDefaults;

public static class ServiceDiscovery
{
    extension(IConfiguration configuration)
    {
        public Uri ResolveServiceUri(ApiDefinition apiDefinition)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(apiDefinition);

            return configuration.ResolveServiceUri(
                apiDefinition.AspireServiceName,
                apiDefinition.AccessOriginConfigurationPath);
        }

        public Uri ResolveServiceUri(string aspireServiceName,
            string? configurationKey = null)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            if (string.IsNullOrWhiteSpace(aspireServiceName))
                throw new ArgumentException("Service name must be provided.", nameof(aspireServiceName));

            var effectiveConfigurationKey = configurationKey ??
                                            $"Apis:AccessOrigins:{ToAccessOriginKeySegment(aspireServiceName)}";
            var configuredValue = configuration[effectiveConfigurationKey];
            if (!string.IsNullOrWhiteSpace(configuredValue))
                return CreateUri(configuredValue, effectiveConfigurationKey);

            return new Uri($"http://{aspireServiceName}", UriKind.Absolute);
        }
    }

    private static Uri CreateUri(string value, string key)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            throw new InvalidOperationException(
                $"Configuration value '{value}' for '{key}' is not a valid absolute URI.");

        return uri;
    }

    private static string ToAccessOriginKeySegment(string aspireServiceName)
    {
        var normalizedServiceName = aspireServiceName.EndsWith("-api", StringComparison.OrdinalIgnoreCase)
            ? aspireServiceName[..^4]
            : aspireServiceName;

        return ToPascalCase(normalizedServiceName);
    }

    private static string ToPascalCase(string value)
    {
        var parts = value.Split(['-', '_'], StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Select(static part =>
        {
            return part.Length switch
            {
                0 => string.Empty,
                1 => part.ToUpperInvariant(),
                _ => char.ToUpperInvariant(part[0]) + part[1..].ToLowerInvariant()
            };
        }));
    }
}