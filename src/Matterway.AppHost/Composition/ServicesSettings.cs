using Microsoft.Extensions.Configuration;

namespace Matterway.AppHost.Composition;

internal sealed class ServicesSettings
{
    public required AspireDashboardServiceEndpoint AspireDashboard { get; init; }
    public required ServiceEndpoint Postgres { get; init; }
    public required MinioServiceEndpoint Minio { get; init; }
    public required ServiceEndpoint Identity { get; init; }
    public required ServiceEndpoint Catalog { get; init; }
    public required ServiceEndpoint Customers { get; init; }
    public required ServiceEndpoint Sales { get; init; }
    public required ServiceEndpoint Storefront { get; init; }
    public required ServiceEndpoint Dashboard { get; init; }

    public static ServicesSettings Bind(IConfiguration configuration)
    {
        var servicesSection = configuration.GetSection("Services");
        if (!servicesSection.Exists())
            throw new InvalidOperationException("Services configuration is missing.");

        return new ServicesSettings
        {
            AspireDashboard = BindEndpoint<AspireDashboardServiceEndpoint>(servicesSection, "AspireDashboard",
                endpoint =>
                {
                    if (endpoint.OtlpPort <= 0)
                        throw new InvalidOperationException(
                            "Services:AspireDashboard:OtlpPort must be greater than zero.");
                }),
            Postgres = BindEndpoint<ServiceEndpoint>(servicesSection, "Postgres"),
            Minio = BindEndpoint<MinioServiceEndpoint>(servicesSection, "Minio",
                endpoint =>
                {
                    if (endpoint.ConsolePort <= 0)
                        throw new InvalidOperationException("Services:Minio:ConsolePort must be greater than zero.");
                }),
            Identity = BindEndpoint<ServiceEndpoint>(servicesSection, "Identity"),
            Catalog = BindEndpoint<ServiceEndpoint>(servicesSection, "Catalog"),
            Customers = BindEndpoint<ServiceEndpoint>(servicesSection, "Customers"),
            Sales = BindEndpoint<ServiceEndpoint>(servicesSection, "Sales"),
            Storefront = BindEndpoint<ServiceEndpoint>(servicesSection, "Storefront"),
            Dashboard = BindEndpoint<ServiceEndpoint>(servicesSection, "Dashboard")
        };
    }

    private static TEndpoint BindEndpoint<TEndpoint>(
        IConfiguration servicesSection,
        string name,
        Action<TEndpoint>? validateExtra = null)
        where TEndpoint : ServiceEndpoint
    {
        var path = $"Services:{name}";
        var section = servicesSection.GetSection(name);
        if (!section.Exists())
            throw new InvalidOperationException($"Services configuration section '{path}' is missing.");

        var endpoint = section.Get<TEndpoint>();
        if (endpoint is null)
            throw new InvalidOperationException($"Services configuration section '{path}' is missing.");

        ValidateEndpoint(endpoint, path);
        validateExtra?.Invoke(endpoint);
        return endpoint;
    }

    private static void ValidateEndpoint(ServiceEndpoint endpoint, string path)
    {
        if (string.IsNullOrWhiteSpace(endpoint.Domain))
            throw new InvalidOperationException($"Services configuration value '{path}:Domain' is missing.");

        if (endpoint.Port <= 0)
            throw new InvalidOperationException(
                $"Services configuration value '{path}:Port' must be greater than zero.");

        if (!string.IsNullOrWhiteSpace(endpoint.Scheme) && !Uri.CheckSchemeName(endpoint.Scheme))
            throw new InvalidOperationException(
                $"Services configuration value '{path}:Scheme' is not a valid URI scheme.");
    }
}

internal class ServiceEndpoint
{
    public string Domain { get; init; } = string.Empty;
    public int Port { get; init; }
    public string Scheme { get; init; } = "http";

    public Uri ToUri()
    {
        return new UriBuilder(string.IsNullOrWhiteSpace(Scheme) ? Uri.UriSchemeHttp : Scheme, Domain, Port).Uri;
    }

    public string ToBaseUrl()
    {
        return ToUri().GetLeftPart(UriPartial.Authority);
    }
}

internal sealed class MinioServiceEndpoint : ServiceEndpoint
{
    public int ConsolePort { get; init; }
}

internal sealed class AspireDashboardServiceEndpoint : ServiceEndpoint
{
    public int OtlpPort { get; init; }
}