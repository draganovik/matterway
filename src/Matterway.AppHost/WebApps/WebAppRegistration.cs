namespace Matterway.AppHost.WebApps;

internal static class WebAppRegistration
{
    public static void Add(
        IDistributedApplicationBuilder builder,
        WebAppOptions options)
    {
        const string httpEndpoint = "http";

        if (builder.ExecutionContext.IsPublishMode)
        {
            var publishedWebApp = builder.AddDockerfile(options.ServiceName, options.SourcePath)
                .WithEnvironment("PORT", options.HostPort.ToString())
                .WithHttpEndpoint(options.HostPort, options.HostPort, httpEndpoint)
                .WithExternalHttpEndpoints();

            foreach (var dependency in options.Dependencies)
                publishedWebApp = publishedWebApp.WaitFor(dependency);

            ApplyConfiguredEnvironment(publishedWebApp, options.ConfigureEnvironment)
                .PublishAsDockerComposeService((_, service) =>
                {
                    service.Restart = "unless-stopped";
                    service.Ports = [$"{options.HostPort}:{options.HostPort}"];
                });

            return;
        }

        var developmentWebApp = builder.AddViteApp(options.ServiceName, options.SourcePath)
            .WithEnvironment("HOST", "localhost")
            .WithEnvironment("PORT", options.HostPort.ToString())
            .WithEndpoint(httpEndpoint, endpoint =>
            {
                endpoint.Port = options.HostPort;
                endpoint.IsProxied = false;
            })
            .WithExternalHttpEndpoints()
            .PublishAsDockerFile();

        foreach (var dependency in options.Dependencies)
            developmentWebApp = developmentWebApp.WaitFor(dependency);

        ApplyConfiguredEnvironment(developmentWebApp, options.ConfigureEnvironment)
            .PublishAsDockerComposeService((_, service) =>
            {
                service.Restart = "unless-stopped";
                service.Ports = [$"{options.HostPort}:{options.HostPort}"];
            });
    }

    private static IResourceBuilder<T> ApplyConfiguredEnvironment<T>(
        IResourceBuilder<T> resource,
        Action<WebAppEnvironment>? configure)
        where T : IResource, IResourceWithEnvironment
    {
        if (configure is null)
            return resource;

        var environment = new WebAppEnvironment();
        configure(environment);
        return environment.ApplyTo(resource);
    }
}