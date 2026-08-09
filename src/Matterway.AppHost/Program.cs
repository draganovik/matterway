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
var minioUser = builder.AddParameter("MinioRootUser");
var minioPassword = builder.AddParameter("MinioRootPassword", true);
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

var minio = builder.AddContainer("minio", "minio/minio:RELEASE.2025-01-20T14-49-07Z")
    .WithVolume("matterway-minio-data", "/data")
    .WithEnvironment("MINIO_ROOT_USER", minioUser)
    .WithEnvironment("MINIO_ROOT_PASSWORD", minioPassword)
    .WithArgs("server", "/data", "--console-address", ":9001")
    .WithHttpEndpoint(services.Minio.Port, 9000, "http")
    .WithHttpEndpoint(services.Minio.ConsolePort, 9001, "console")
    .PublishAsDockerComposeService((_, service) => { service.Restart = "unless-stopped"; });

var dbMigrator = builder.AddProject<Matterway_Migrations>("mtw-db-migrator")
    .WithReference(databases.Catalog)
    .WithReference(databases.Customers)
    .WithReference(databases.Identity)
    .WithReference(databases.Sales)
    .WaitFor(postgres)
    .PublishAsDockerComposeService((_, service) => { service.Restart = "no"; });

var identityApi = AddApi<Matterway_Identity_Api>(
    ApiDirectory.Identity,
    api => api.WithReference(databases.Identity));

var catalogApi = AddApi<Matterway_Catalog_Api>(
    ApiDirectory.Catalog,
    api => api.WithReference(databases.Catalog));

var customersApi = AddApi<Matterway_Customers_Api>(
    ApiDirectory.Customers,
    api => api
        .WithReference(databases.Customers)
        .WithReference(identityApi.GetEndpoint("http"))
        .WithReference(catalogApi.GetEndpoint("http")));

var salesApi = AddApi<Matterway_Sales_Api>(
    ApiDirectory.Sales,
    api => api.WithReference(databases.Sales)
        .WithReference(customersApi.GetEndpoint("http")));

var identityApiHttp = identityApi.GetEndpoint("http");
var catalogApiHttp = catalogApi.GetEndpoint("http");
var customersApiHttp = customersApi.GetEndpoint("http");
var salesApiHttp = salesApi.GetEndpoint("http");
var minioHttpEndpoint = minio.GetEndpoint("http");

catalogApi
    .WaitFor(minio)
    .WithReference(minioHttpEndpoint)
    .WithEnvironment("ImageStorage__Bucket", "article-images")
    .WithEnvironment("ImageStorage__Endpoint", minioHttpEndpoint)
    .WithEnvironment("ImageStorage__AccessKey", minioUser)
    .WithEnvironment("ImageStorage__SecretKey", minioPassword);

var apiResources = new (ApiDefinition Definition, IResourceBuilder<ProjectResource> Resource)[]
{
    (ApiDirectory.Identity, identityApi),
    (ApiDirectory.Catalog, catalogApi),
    (ApiDirectory.Customers, customersApi),
    (ApiDirectory.Sales, salesApi)
};

#pragma warning disable ASPIREJAVASCRIPT001
WithCommonWebEnvironment(
        builder.AddViteApp("mtw-storefront-web", "../Matterway.Storefront.Web")
            .PublishAsPackageScript()
            .WithEndpoint("http", endpoint =>
            {
                endpoint.Port = services.Storefront.Port;
                endpoint.IsProxied = false;
            })
            .WithExternalHttpEndpoints())
    .WithEnvironment("NUXT_SYSTEM_ACCESS_KEY", systemAccessKey)
    .WithEnvironment("NUXT_STRIPE_SECRET_KEY", stripeSecretKey)
    .WaitFor(catalogApi)
    .WaitFor(salesApi)
    .PublishAsDockerComposeService((_, service) => { service.Restart = "unless-stopped"; });

WithCommonWebEnvironment(
        builder.AddViteApp("mtw-dashboard-web", "../Matterway.Dashboard.Web")
            .PublishAsPackageScript()
            .WithEndpoint("http", endpoint =>
            {
                endpoint.Port = services.Dashboard.Port;
                endpoint.IsProxied = false;
            })
            .WithExternalHttpEndpoints())
    .WaitFor(identityApi)
    .WaitFor(catalogApi)
    .WaitFor(customersApi)
    .WaitFor(salesApi)
    .PublishAsDockerComposeService((_, service) => { service.Restart = "unless-stopped"; });
#pragma warning restore ASPIREJAVASCRIPT001

if (builder.Environment.IsDevelopment())
    ScalarApiRegistration.AddApiReferences(builder, services.Scalar.Port, apiResources);

foreach (var (_, api) in apiResources)
    api
        .WithEnvironment("Jwt__Key", jwtSigningKey)
        .WithEnvironment("Jwt__Issuer", identityApiHttp)
        .WithEnvironment("Jwt__Audience", identityApiHttp)
        .WithEnvironment("Security__SystemAccessKey", systemAccessKey);

builder.Build().Run();

IResourceBuilder<ProjectResource> AddApi<TProject>(
    ApiDefinition apiDefinition,
    Func<IResourceBuilder<ProjectResource>, IResourceBuilder<ProjectResource>> configure)
    where TProject : IProjectMetadata, new()
{
    var api = builder.AddProject<TProject>(apiDefinition.AspireServiceName)
        .PublishAsDockerComposeService((_, service) => { service.Restart = "unless-stopped"; });

    if (!builder.ExecutionContext.IsPublishMode)
        api = api.WithExternalHttpEndpoints();

    return configure(api).WaitFor(dbMigrator);
}

IResourceBuilder<T> WithCommonWebEnvironment<T>(IResourceBuilder<T> webApp)
    where T : IResource, IResourceWithEnvironment
{
    if (!builder.ExecutionContext.IsPublishMode)
        webApp = webApp.WithEnvironment("HOST", "localhost");

    return webApp
        .WithEnvironment("NUXT_SERVER_IDENTITY_API_BASE_URL", identityApiHttp)
        .WithEnvironment("NUXT_SERVER_CATALOG_API_BASE_URL", catalogApiHttp)
        .WithEnvironment("NUXT_SERVER_CUSTOMERS_API_BASE_URL", customersApiHttp)
        .WithEnvironment("NUXT_SERVER_SALES_API_BASE_URL", salesApiHttp)
        .WithEnvironment("NUXT_SERVER_IMAGE_CDN_BASE_URL", minioHttpEndpoint)
        .WithEnvironment("NUXT_SERVER_IMAGE_CDN_BUCKET", "article-images");
}
