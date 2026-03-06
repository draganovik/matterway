namespace Matterway.AppHost.Composition;

internal sealed class WebAppCompositionOptions
{
    public required string ServiceName { get; init; }
    public required string RelativePath { get; init; }
    public required int HostPort { get; init; }
    public required IReadOnlyList<IResourceBuilder<ProjectResource>> Dependencies { get; init; }
    public Action<WebAppEnvironmentBuilder>? ConfigureEnvironment { get; init; }
}

internal static class WebAppComposition
{
    public static void AddWebApp(
        IDistributedApplicationBuilder builder,
        WebAppCompositionOptions options)
    {
        const string httpEndpoint = "http";

        if (builder.ExecutionContext.IsPublishMode)
        {
            var published = builder.AddDockerfile(options.ServiceName, options.RelativePath)
                .WithEnvironment("PORT", options.HostPort.ToString())
                .WithHttpEndpoint(options.HostPort, options.HostPort, httpEndpoint);

            foreach (var dependency in options.Dependencies)
                published = published.WaitFor(dependency);

            ApplyConfiguredEnvironment(published, options.ConfigureEnvironment)
                .PublishAsDockerComposeService((_, service) =>
                {
                    service.Restart = "unless-stopped";
                    service.Ports = [$"{options.HostPort}:{options.HostPort}"];
                });

            return;
        }

        var development = builder.AddViteApp(options.ServiceName, options.RelativePath)
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
            development = development.WaitFor(dependency);

        ApplyConfiguredEnvironment(development, options.ConfigureEnvironment)
            .PublishAsDockerComposeService((_, service) =>
            {
                service.Restart = "unless-stopped";
                service.Ports = [$"{options.HostPort}:{options.HostPort}"];
            });
    }

    private static IResourceBuilder<T> ApplyConfiguredEnvironment<T>(
        IResourceBuilder<T> resource,
        Action<WebAppEnvironmentBuilder>? configure)
        where T : IResource, IResourceWithEnvironment
    {
        if (configure is null)
            return resource;

        var environment = new WebAppEnvironmentBuilder();
        configure(environment);
        return environment.Apply(resource);
    }
}

internal sealed class WebAppEnvironmentBuilder
{
    private readonly List<EnvironmentEntry> _entries = [];

    public WebAppEnvironmentBuilder WithEnvironment(string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _entries.Add(EnvironmentEntry.FromString(name, value));
        return this;
    }

    public WebAppEnvironmentBuilder WithTrimmedEnvironment(string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _entries.Add(EnvironmentEntry.FromString(name, value.TrimEnd('/')));
        return this;
    }

    public WebAppEnvironmentBuilder WithEnvironment(string name, EndpointReference value)
    {
        _entries.Add(EnvironmentEntry.FromEndpoint(name, value));
        return this;
    }

    public WebAppEnvironmentBuilder WithEnvironment(string name, IResourceBuilder<ParameterResource> value)
    {
        _entries.Add(EnvironmentEntry.FromParameter(name, value));
        return this;
    }

    public IResourceBuilder<T> Apply<T>(IResourceBuilder<T> resource)
        where T : IResource, IResourceWithEnvironment
    {
        foreach (var entry in _entries)
            resource = entry switch
            {
                { Endpoint: not null } => resource.WithEnvironment(entry.Name, entry.Endpoint),
                { Parameter: not null } => resource.WithEnvironment(entry.Name, entry.Parameter),
                _ => resource.WithEnvironment(entry.Name, entry.StringValue!)
            };

        return resource;
    }

    private sealed record EnvironmentEntry(
        string Name,
        string? StringValue,
        EndpointReference? Endpoint,
        IResourceBuilder<ParameterResource>? Parameter)
    {
        public static EnvironmentEntry FromString(string name, string value)
        {
            return new EnvironmentEntry(name, value, null, null);
        }

        public static EnvironmentEntry FromEndpoint(string name, EndpointReference value)
        {
            return new EnvironmentEntry(name, null, value, null);
        }

        public static EnvironmentEntry FromParameter(string name, IResourceBuilder<ParameterResource> value)
        {
            return new EnvironmentEntry(name, null, null, value);
        }
    }
}