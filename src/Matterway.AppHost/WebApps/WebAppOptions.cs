namespace Matterway.AppHost.WebApps;

internal sealed class WebAppOptions
{
    public required string ServiceName { get; init; }
    public required string SourcePath { get; init; }
    public required int HostPort { get; init; }
    public required IReadOnlyList<IResourceBuilder<ProjectResource>> Dependencies { get; init; }
    public Action<WebAppEnvironment>? ConfigureEnvironment { get; init; }
}