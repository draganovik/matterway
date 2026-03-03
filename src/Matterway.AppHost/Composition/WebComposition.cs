namespace Matterway.AppHost.Composition;

internal sealed record WebEndpoints(
    EndpointReference StorefrontHttpEndpoint,
    EndpointReference DashboardHttpEndpoint);

internal static class WebComposition
{
    private const string HttpEndpoint = "http";
    private const int StorefrontPort = 3001;
    private const int DashboardPort = 3002;
    private const string RestartPolicy = "unless-stopped";

    public static WebEndpoints AddWebApps(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> identityApi,
        IResourceBuilder<ProjectResource> catalogApi,
        IResourceBuilder<ProjectResource> customersApi,
        IResourceBuilder<ProjectResource> salesApi)
    {
        var storefront = builder.ExecutionContext.IsPublishMode
            ? AddPublishedStorefront(builder, catalogApi, salesApi)
            : AddDevelopmentStorefront(builder, catalogApi, salesApi);
        var dashboard = builder.ExecutionContext.IsPublishMode
            ? AddPublishedDashboard(builder, identityApi, catalogApi, customersApi, salesApi)
            : AddDevelopmentDashboard(builder, identityApi, catalogApi, customersApi, salesApi);
        return new WebEndpoints(storefront, dashboard);
    }

    private static EndpointReference AddDevelopmentStorefront(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> catalogApi,
        IResourceBuilder<ProjectResource> salesApi)
    {
        var resource = builder.AddViteApp("storefront-web", "../Matterway.Storefront.Web")
            .WaitFor(catalogApi)
            .WaitFor(salesApi)
            .WithEnvironment("NUXT_SERVER_SALES_API_BASE_URL", salesApi.GetEndpoint(HttpEndpoint))
            .WithEnvironment("PORT", StorefrontPort.ToString())
            .WithEndpoint(HttpEndpoint, endpoint =>
            {
                endpoint.Port = StorefrontPort;
                endpoint.IsProxied = false;
            })
            .WithExternalHttpEndpoints()
            .PublishAsDockerFile();
        return PublishForCompose(resource, StorefrontPort).GetEndpoint(HttpEndpoint);
    }

    private static EndpointReference AddPublishedStorefront(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> catalogApi,
        IResourceBuilder<ProjectResource> salesApi)
    {
        var resource = builder.AddDockerfile("storefront-web", "../Matterway.Storefront.Web")
            .WaitFor(catalogApi)
            .WaitFor(salesApi)
            .WithEnvironment("NUXT_SERVER_SALES_API_BASE_URL", salesApi.GetEndpoint(HttpEndpoint))
            .WithEnvironment("PORT", StorefrontPort.ToString())
            .WithHttpEndpoint(StorefrontPort, StorefrontPort, HttpEndpoint);
        return PublishForCompose(resource, StorefrontPort).GetEndpoint(HttpEndpoint);
    }

    private static EndpointReference AddDevelopmentDashboard(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> identityApi,
        IResourceBuilder<ProjectResource> catalogApi,
        IResourceBuilder<ProjectResource> customersApi,
        IResourceBuilder<ProjectResource> salesApi)
    {
        var resource = builder.AddViteApp("dashboard-web", "../Matterway.Dashboard.Web")
            .WaitFor(identityApi)
            .WaitFor(catalogApi)
            .WaitFor(customersApi)
            .WaitFor(salesApi)
            .WithEnvironment("PORT", DashboardPort.ToString())
            .WithEndpoint(HttpEndpoint, endpoint =>
            {
                endpoint.Port = DashboardPort;
                endpoint.IsProxied = false;
            })
            .WithExternalHttpEndpoints()
            .PublishAsDockerFile();
        resource = WithDashboardApiEnvs(resource, identityApi, catalogApi, customersApi, salesApi);
        return PublishForCompose(resource, DashboardPort).GetEndpoint(HttpEndpoint);
    }

    private static EndpointReference AddPublishedDashboard(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> identityApi,
        IResourceBuilder<ProjectResource> catalogApi,
        IResourceBuilder<ProjectResource> customersApi,
        IResourceBuilder<ProjectResource> salesApi)
    {
        var resource = builder.AddDockerfile("dashboard-web", "../Matterway.Dashboard.Web")
            .WaitFor(identityApi)
            .WaitFor(catalogApi)
            .WaitFor(customersApi)
            .WaitFor(salesApi)
            .WithEnvironment("PORT", DashboardPort.ToString())
            .WithHttpEndpoint(DashboardPort, DashboardPort, HttpEndpoint);
        resource = WithDashboardApiEnvs(resource, identityApi, catalogApi, customersApi, salesApi);
        return PublishForCompose(resource, DashboardPort).GetEndpoint(HttpEndpoint);
    }

    private static IResourceBuilder<T> WithDashboardApiEnvs<T>(
        IResourceBuilder<T> resource,
        IResourceBuilder<ProjectResource> identityApi,
        IResourceBuilder<ProjectResource> catalogApi,
        IResourceBuilder<ProjectResource> customersApi,
        IResourceBuilder<ProjectResource> salesApi)
        where T : IResource, IResourceWithEnvironment
    {
        foreach (var (prefix, endpoint) in new[]
                 {
                     ("IDENTITY", identityApi.GetEndpoint(HttpEndpoint)),
                     ("CATALOG", catalogApi.GetEndpoint(HttpEndpoint)),
                     ("CUSTOMERS", customersApi.GetEndpoint(HttpEndpoint)),
                     ("SALES", salesApi.GetEndpoint(HttpEndpoint))
                 })
            resource = resource.WithEnvironment($"NUXT_SERVER_{prefix}_API_BASE_URL", endpoint)
                .WithEnvironment($"{prefix}_API_BASE_URL", endpoint);
        return resource;
    }

    private static IResourceBuilder<T> PublishForCompose<T>(IResourceBuilder<T> resource, int port)
        where T : IResource, IComputeResource
    {
        return resource.PublishAsDockerComposeService((_, service) =>
        {
            service.Restart = RestartPolicy;
            service.Ports = [$"{port}:{port}"];
        });
    }
}