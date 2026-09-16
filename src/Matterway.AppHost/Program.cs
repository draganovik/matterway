using Aspire.Hosting.JavaScript;
using Matterway.AppHost.ApiDocumentation;
using Matterway.AppHost.Configuration;
using Matterway.ServiceDefaults;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions
{
    Args = args,
    DashboardApplicationName = "Matterway Aspire"
});
const string composeEnvironmentName = "matterway-platform";
var services = AppHostServicesOptions.Bind(builder.Configuration);

var jwtSigningKey = builder.AddParameter("JwtSigningKey", true);
var systemAccessKey = builder.AddParameter("SystemAccessKey", true);
var postgresPassword = builder.AddParameter("PostgresPassword", true);
var rustfsAccessKey = builder.AddParameter("RustFSAccessKey");
var rustfsSecretKey = builder.AddParameter("RustFSSecretKey", true);
var stripeSecretKey = builder.AddParameter("StripeSecretKey", true);

builder.AddDockerComposeEnvironment(composeEnvironmentName)
    .WithDashboard(dashboard =>
    {
        if (builder.Environment.IsDevelopment())
            dashboard.WithHostPort(services.AspireDashboard.Port);
        dashboard.WithContainerName("aspire-dashboard");
    })
    .ConfigureComposeFile(compose => compose.Name = composeEnvironmentName);

var postgres = builder.AddPostgres("postgres")
    .WithImageTag("18")
    .WithHostPort(services.Postgres.Port)
    .WithPassword(postgresPassword)
    .WithVolume("matterway-postgres-data", "/var/lib/postgresql")
    .PublishAsDockerComposeService((_, service) => { service.Restart = "unless-stopped"; });

var databases = (
    Catalog: postgres.AddDatabase("CatalogDb"),
    Customers: postgres.AddDatabase("CustomersDb"),
    Identity: postgres.AddDatabase("IdentityDb"),
    Sales: postgres.AddDatabase("SalesDb"));

var rustfs = builder.AddContainer("rustfs", "rustfs/rustfs:1.0.0-rc.6")
    .WithVolume("matterway-rustfs-data", "/data")
    .WithEnvironment("RUSTFS_ACCESS_KEY", rustfsAccessKey)
    .WithEnvironment("RUSTFS_SECRET_KEY", rustfsSecretKey)
    .WithEnvironment("RUSTFS_CONSOLE_ENABLE", "true")
    .WithEnvironment("RUSTFS_ADDRESS", ":9000")
    .WithEnvironment("RUSTFS_CONSOLE_ADDRESS", ":9001")
    .WithArgs("/data")
    .WithHttpEndpoint(services.RustFS.Port, 9000, "http")
    .WithHttpEndpoint(services.RustFS.ConsolePort, 9001, "console")
    .WithUrlForEndpoint("console", url => url.Url = "/rustfs/console/")
    .WithHttpHealthCheck("/health/ready", endpointName: "http")
    .PublishAsDockerComposeService((_, service) => { service.Restart = "unless-stopped"; });

var dbMigrator = builder.AddProject<Matterway_Migrations>("mtw-db-migrator")
    .WithReference(databases.Catalog)
    .WithReference(databases.Customers)
    .WithReference(databases.Identity)
    .WithReference(databases.Sales)
    .WaitFor(postgres)
    .PublishAsDockerComposeService((_, service) => { service.Restart = "no"; });

var identityApi = AddApi<Matterway_Identity_Api>(ApiDirectory.Identity)
    .WithReference(databases.Identity);

var catalogApi = AddApi<Matterway_Catalog_Api>(ApiDirectory.Catalog)
    .WithReference(databases.Catalog)
    .WaitFor(rustfs)
    .WithReference(rustfs.GetEndpoint("http"))
    .WithEnvironment("ImageStorage__Bucket", "article-images")
    .WithEnvironment("ImageStorage__Endpoint", rustfs.GetEndpoint("http"))
    .WithEnvironment("ImageStorage__AccessKey", rustfsAccessKey)
    .WithEnvironment("ImageStorage__SecretKey", rustfsSecretKey);

var customersApi = AddApi<Matterway_Customers_Api>(ApiDirectory.Customers)
    .WithReference(databases.Customers)
    .WithReference(identityApi.GetEndpoint("http"))
    .WithReference(catalogApi.GetEndpoint("http"));

var salesApi = AddApi<Matterway_Sales_Api>(ApiDirectory.Sales)
    .WithReference(databases.Sales)
    .WithReference(customersApi.GetEndpoint("http"));

var apiResources = new (ApiDefinition Definition, IResourceBuilder<ProjectResource> Resource)[]
{
    (ApiDirectory.Identity, identityApi),
    (ApiDirectory.Catalog, catalogApi),
    (ApiDirectory.Customers, customersApi),
    (ApiDirectory.Sales, salesApi)
};

foreach (var (_, api) in apiResources)
    api
        .WithEnvironment("Jwt__Key", jwtSigningKey)
        .WithEnvironment("Jwt__Issuer", identityApi.GetEndpoint("http"))
        .WithEnvironment("Jwt__Audience", identityApi.GetEndpoint("http"))
        .WithEnvironment("Security__SystemAccessKey", systemAccessKey);

AddWebApp("mtw-storefront-web", "../Matterway.Storefront.Web", services.Storefront.Port)
    .WithEnvironment("NUXT_SYSTEM_ACCESS_KEY", systemAccessKey)
    .WithEnvironment("NUXT_STRIPE_SECRET_KEY", stripeSecretKey)
    .WaitFor(catalogApi)
    .WaitFor(salesApi);

AddWebApp("mtw-dashboard-web", "../Matterway.Dashboard.Web", services.Dashboard.Port)
    .WaitFor(identityApi)
    .WaitFor(catalogApi)
    .WaitFor(customersApi)
    .WaitFor(salesApi);

if (builder.Environment.IsDevelopment())
    ScalarApiRegistration.AddApiReferences(builder, services.Scalar.Port, apiResources);

builder.Build().Run();

IResourceBuilder<ProjectResource> AddApi<TProject>(ApiDefinition apiDefinition)
    where TProject : IProjectMetadata, new()
{
    var api = builder.AddProject<TProject>(apiDefinition.AspireServiceName)
        .PublishAsDockerComposeService((_, service) => { service.Restart = "unless-stopped"; });

    if (!builder.ExecutionContext.IsPublishMode)
        api = api.WithExternalHttpEndpoints();

    return api.WaitFor(dbMigrator);
}

#pragma warning disable ASPIREJAVASCRIPT001
IResourceBuilder<ViteAppResource> AddWebApp(string name, string path, int port)
{
    var webApp = builder.AddViteApp(name, path)
        .PublishAsPackageScript()
        .WithEndpoint("http", endpoint =>
        {
            endpoint.Port = port;
            endpoint.IsProxied = false;
        })
        .WithExternalHttpEndpoints()
        .PublishAsDockerComposeService((_, service) => { service.Restart = "unless-stopped"; });

    if (!builder.ExecutionContext.IsPublishMode)
        webApp = webApp.WithEnvironment("HOST", "localhost");

    return webApp
        .WithEnvironment("NUXT_SERVER_IDENTITY_API_BASE_URL", identityApi.GetEndpoint("http"))
        .WithEnvironment("NUXT_SERVER_CATALOG_API_BASE_URL", catalogApi.GetEndpoint("http"))
        .WithEnvironment("NUXT_SERVER_CUSTOMERS_API_BASE_URL", customersApi.GetEndpoint("http"))
        .WithEnvironment("NUXT_SERVER_SALES_API_BASE_URL", salesApi.GetEndpoint("http"))
        .WithEnvironment("NUXT_SERVER_IMAGE_CDN_BASE_URL", rustfs.GetEndpoint("http"))
        .WithEnvironment("NUXT_SERVER_IMAGE_CDN_BUCKET", "article-images");
}
#pragma warning restore ASPIREJAVASCRIPT001
