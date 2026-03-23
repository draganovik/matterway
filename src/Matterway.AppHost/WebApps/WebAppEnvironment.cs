namespace Matterway.AppHost.WebApps;

internal sealed class WebAppEnvironment
{
    private readonly List<EnvironmentEntry> _entries = [];

    public WebAppEnvironment WithEnvironment(string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _entries.Add(EnvironmentEntry.FromString(name, value));
        return this;
    }

    public WebAppEnvironment WithTrimmedEnvironment(string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            _entries.Add(EnvironmentEntry.FromString(name, value.TrimEnd('/')));
        return this;
    }

    public WebAppEnvironment WithEnvironment(string name, EndpointReference value)
    {
        _entries.Add(EnvironmentEntry.FromEndpoint(name, value));
        return this;
    }

    public WebAppEnvironment WithEnvironment(string name, IResourceBuilder<ParameterResource> value)
    {
        _entries.Add(EnvironmentEntry.FromParameter(name, value));
        return this;
    }

    public IResourceBuilder<T> ApplyTo<T>(IResourceBuilder<T> resource)
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