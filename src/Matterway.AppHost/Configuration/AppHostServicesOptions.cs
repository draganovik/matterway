using Microsoft.Extensions.Configuration;

namespace Matterway.AppHost.Configuration;

internal sealed class AppHostServicesOptions
{
    public required AspireDashboardPortOptions AspireDashboard { get; init; }
    public required ServicePortOptions Postgres { get; init; }
    public required MinioPortOptions Minio { get; init; }
    public required ServicePortOptions Storefront { get; init; }
    public required ServicePortOptions Dashboard { get; init; }

    public static AppHostServicesOptions Bind(IConfiguration configuration)
    {
        var servicesSection = configuration.GetSection("Services");
        if (!servicesSection.Exists())
            throw new InvalidOperationException("Services configuration is missing.");

        return new AppHostServicesOptions
        {
            AspireDashboard = BindPortOptions<AspireDashboardPortOptions>(servicesSection, "AspireDashboard",
                options =>
                {
                    if (options.OtlpPort <= 0)
                        throw new InvalidOperationException(
                            "Services:AspireDashboard:OtlpPort must be greater than zero.");
                }),
            Postgres = BindPortOptions<ServicePortOptions>(servicesSection, "Postgres"),
            Minio = BindPortOptions<MinioPortOptions>(servicesSection, "Minio",
                options =>
                {
                    if (options.ConsolePort <= 0)
                        throw new InvalidOperationException("Services:Minio:ConsolePort must be greater than zero.");
                }),
            Storefront = BindPortOptions<ServicePortOptions>(servicesSection, "Storefront"),
            Dashboard = BindPortOptions<ServicePortOptions>(servicesSection, "Dashboard")
        };
    }

    private static TOptions BindPortOptions<TOptions>(
        IConfiguration servicesSection,
        string name,
        Action<TOptions>? validateExtra = null)
        where TOptions : ServicePortOptions
    {
        var path = $"Services:{name}";
        var section = servicesSection.GetSection(name);
        if (!section.Exists())
            throw new InvalidOperationException($"Services configuration section '{path}' is missing.");

        var options = section.Get<TOptions>();
        if (options is null)
            throw new InvalidOperationException($"Services configuration section '{path}' is missing.");

        ValidatePortOptions(options, path);
        validateExtra?.Invoke(options);
        return options;
    }

    private static void ValidatePortOptions(ServicePortOptions options, string path)
    {
        if (options.Port <= 0)
            throw new InvalidOperationException(
                $"Services configuration value '{path}:Port' must be greater than zero.");
    }
}

internal class ServicePortOptions
{
    public int Port { get; init; }
}

internal sealed class MinioPortOptions : ServicePortOptions
{
    public int ConsolePort { get; init; }
}

internal sealed class AspireDashboardPortOptions : ServicePortOptions
{
    public int OtlpPort { get; init; }
}