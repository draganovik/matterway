namespace Matterway.AppHost.Composition;

internal sealed record WebEndpoints(
    EndpointReference StorefrontHttpEndpoint,
    EndpointReference DashboardHttpEndpoint);

internal static class WebComposition
{
    public static WebEndpoints AddWebApps(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> identityApi,
        IResourceBuilder<ProjectResource> catalogApi,
        IResourceBuilder<ProjectResource> customersApi,
        IResourceBuilder<ProjectResource> salesApi)
    {
        if (builder.ExecutionContext.IsPublishMode)
            return AddPublishedWebApps(builder, identityApi, catalogApi, customersApi, salesApi);

        return AddDevelopmentWebApps(builder, identityApi, catalogApi, customersApi, salesApi);
    }

    private static WebEndpoints AddDevelopmentWebApps(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> identityApi,
        IResourceBuilder<ProjectResource> catalogApi,
        IResourceBuilder<ProjectResource> customersApi,
        IResourceBuilder<ProjectResource> salesApi)
    {
        var storefront = builder.AddViteApp("storefront-web", "../Matterway.Storefront.Web")
            .WaitFor(catalogApi)
            .WaitFor(salesApi)
            .WithEnvironment("NUXT_SERVER_SALES_API_BASE_URL", salesApi.GetEndpoint("http"))
            .WithEnvironment("PORT", "3001")
            .WithEndpoint("http", endpoint =>
            {
                endpoint.Port = 3001;
                endpoint.IsProxied = false;
            })
            .WithExternalHttpEndpoints()
            .PublishAsDockerFile()
            .PublishAsDockerComposeService((_, service) =>
            {
                service.Restart = "unless-stopped";
                service.Ports = ["3001:3001"];
            });

        var dashboard = builder.AddViteApp("dashboard-web", "../Matterway.Dashboard.Web")
            .WaitFor(identityApi)
            .WaitFor(catalogApi)
            .WaitFor(customersApi)
            .WaitFor(salesApi)
            .WithEnvironment("NUXT_SERVER_IDENTITY_API_BASE_URL", identityApi.GetEndpoint("http"))
            .WithEnvironment("NUXT_SERVER_CATALOG_API_BASE_URL", catalogApi.GetEndpoint("http"))
            .WithEnvironment("NUXT_SERVER_CUSTOMERS_API_BASE_URL", customersApi.GetEndpoint("http"))
            .WithEnvironment("NUXT_SERVER_SALES_API_BASE_URL", salesApi.GetEndpoint("http"))
            .WithEnvironment("IDENTITY_API_BASE_URL", identityApi.GetEndpoint("http"))
            .WithEnvironment("CATALOG_API_BASE_URL", catalogApi.GetEndpoint("http"))
            .WithEnvironment("CUSTOMERS_API_BASE_URL", customersApi.GetEndpoint("http"))
            .WithEnvironment("SALES_API_BASE_URL", salesApi.GetEndpoint("http"))
            .WithEnvironment("PORT", "3002")
            .WithEndpoint("http", endpoint =>
            {
                endpoint.Port = 3002;
                endpoint.IsProxied = false;
            })
            .WithExternalHttpEndpoints()
            .PublishAsDockerFile()
            .PublishAsDockerComposeService((_, service) =>
            {
                service.Restart = "unless-stopped";
                service.Ports = ["3002:3002"];
            });

        return new WebEndpoints(
            storefront.GetEndpoint("http"),
            dashboard.GetEndpoint("http"));
    }

    private static WebEndpoints AddPublishedWebApps(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> identityApi,
        IResourceBuilder<ProjectResource> catalogApi,
        IResourceBuilder<ProjectResource> customersApi,
        IResourceBuilder<ProjectResource> salesApi)
    {
        var storefront = builder.AddDockerfile("storefront-web", "../Matterway.Storefront.Web")
            .WaitFor(catalogApi)
            .WaitFor(salesApi)
            .WithEnvironment("NUXT_SERVER_SALES_API_BASE_URL", salesApi.GetEndpoint("http"))
            .WithEnvironment("PORT", "3001")
            .WithHttpEndpoint(3001, 3001, "http")
            .PublishAsDockerComposeService((_, service) =>
            {
                service.Restart = "unless-stopped";
                service.Ports = ["3001:3001"];
            });

        var dashboard = builder.AddDockerfile("dashboard-web", "../Matterway.Dashboard.Web")
            .WaitFor(identityApi)
            .WaitFor(catalogApi)
            .WaitFor(customersApi)
            .WaitFor(salesApi)
            .WithEnvironment("NUXT_SERVER_IDENTITY_API_BASE_URL", identityApi.GetEndpoint("http"))
            .WithEnvironment("NUXT_SERVER_CATALOG_API_BASE_URL", catalogApi.GetEndpoint("http"))
            .WithEnvironment("NUXT_SERVER_CUSTOMERS_API_BASE_URL", customersApi.GetEndpoint("http"))
            .WithEnvironment("NUXT_SERVER_SALES_API_BASE_URL", salesApi.GetEndpoint("http"))
            .WithEnvironment("IDENTITY_API_BASE_URL", identityApi.GetEndpoint("http"))
            .WithEnvironment("CATALOG_API_BASE_URL", catalogApi.GetEndpoint("http"))
            .WithEnvironment("CUSTOMERS_API_BASE_URL", customersApi.GetEndpoint("http"))
            .WithEnvironment("SALES_API_BASE_URL", salesApi.GetEndpoint("http"))
            .WithEnvironment("PORT", "3002")
            .WithHttpEndpoint(3002, 3002, "http")
            .PublishAsDockerComposeService((_, service) =>
            {
                service.Restart = "unless-stopped";
                service.Ports = ["3002:3002"];
            });

        return new WebEndpoints(
            storefront.GetEndpoint("http"),
            dashboard.GetEndpoint("http"));
    }
}