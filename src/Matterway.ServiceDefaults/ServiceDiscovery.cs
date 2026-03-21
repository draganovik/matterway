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

            var aspireServiceName = apiDefinition.AspireServiceName;
            var configurationPath = apiDefinition.ServiceConfigurationPath;
            var serviceSection = configuration.GetSection(configurationPath);

            if (!serviceSection.Exists())
                return new Uri($"http://{aspireServiceName}", UriKind.Absolute);

            var domain = serviceSection["Domain"];
            var portText = serviceSection["Port"];
            if (string.IsNullOrWhiteSpace(domain))
                throw new InvalidOperationException(
                    $"Configuration value '{configurationPath}:Domain' is required.");
            if (string.IsNullOrWhiteSpace(portText))
                throw new InvalidOperationException(
                    $"Configuration value '{configurationPath}:Port' is required.");

            if (!int.TryParse(portText, out var port) || port <= 0)
                throw new InvalidOperationException(
                    $"Configuration value '{configurationPath}:Port' must be a positive integer.");

            var scheme = serviceSection["Scheme"];
            if (string.IsNullOrWhiteSpace(scheme))
                scheme = Uri.UriSchemeHttp;

            if (!Uri.CheckSchemeName(scheme))
                throw new InvalidOperationException(
                    $"Configuration value '{configurationPath}:Scheme' is not a valid URI scheme.");

            return new UriBuilder(scheme, domain.Trim(), port).Uri;
        }
    }
}