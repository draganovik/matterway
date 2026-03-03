using Matterway.AppHost.Composition;
using Matterway.ServiceDefaults;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var jwtSigningKey = builder.AddParameter("JwtSigningKey", true);
var postgresPassword = builder.AddParameter("PostgresPassword", true);
var minioUser = builder.AddParameter("MinioRootUser");
var minioPassword = builder.AddParameter("MinioRootPassword", true);
var stripeSecretKey = builder.AddParameter("StripeSecretKey", true);

builder.AddDockerComposeEnvironment("matterway-platform")
    .WithDashboard(dashboard =>
    {
        dashboard.WithHostPort(PlatformPorts.AspireDashboard);
        dashboard.WithContainerName("aspire-dashboard");
    })
    .ConfigureComposeFile(compose => compose.Name = "matterway-erp-stack");

var postgres = builder.AddPostgres("postgres")
    .WithImageTag("18")
    .WithHostPort(PlatformPorts.Postgres)
    .WithPassword(postgresPassword)
    .WithVolume("matterway-postgres-data", "/var/lib/postgresql")
    .WithBindMount("../../data", "/seed-data", true)
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = [$"{PlatformPorts.Postgres}:5432"];
    });

var databases = (
    Catalog: postgres.AddDatabase("CatalogDb"),
    Customers: postgres.AddDatabase("CustomersDb"),
    Identity: postgres.AddDatabase("IdentityDb"),
    Sales: postgres.AddDatabase("SalesDb"));

var minio = builder.AddContainer("minio", "minio/minio:latest")
    .WithVolume("matterway-minio-data", "/data")
    .WithEnvironment("MINIO_ROOT_USER", minioUser)
    .WithEnvironment("MINIO_ROOT_PASSWORD", minioPassword)
    .WithArgs("server", "/data", "--console-address", ":9001")
    .WithHttpEndpoint(PlatformPorts.MinioApi, 9000, "http")
    .WithHttpEndpoint(PlatformPorts.MinioConsole, 9001, "console")
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = [$"{PlatformPorts.MinioApi}:9000", $"{PlatformPorts.MinioConsole}:9001"];
    });

builder.AddProject<Matterway_Migrations>("db-migrator")
    .WithReference(databases.Catalog)
    .WithReference(databases.Customers)
    .WithReference(databases.Identity)
    .WithReference(databases.Sales)
    .WaitFor(postgres)
    .WithExplicitStart()
    .PublishAsDockerComposeService((_, service) => { service.Restart = "no"; });

var identityApi = AddApi<Matterway_Identity_Api>(
    ApiDirectory.Identity,
    PlatformPorts.IdentityApi,
    api => api.WithReference(databases.Identity));

var catalogApi = AddApi<Matterway_Catalog_Api>(
    ApiDirectory.Catalog,
    PlatformPorts.CatalogApi,
    api => api.WithReference(databases.Catalog));

var customersApi = AddApi<Matterway_Customers_Api>(
    ApiDirectory.Customers,
    PlatformPorts.CustomersApi,
    api => api
        .WithReference(databases.Customers)
        .WithReference(identityApi.GetEndpoint("http"))
        .WithReference(catalogApi.GetEndpoint("http")));

var salesApi = AddApi<Matterway_Sales_Api>(
    ApiDirectory.Sales,
    PlatformPorts.SalesApi,
    api => api.WithReference(databases.Sales)
        .WithReference(customersApi.GetEndpoint("http")));

var identityApiHttp = identityApi.GetEndpoint("http");
var catalogApiHttp = catalogApi.GetEndpoint("http");
var customersApiHttp = customersApi.GetEndpoint("http");
var salesApiHttp = salesApi.GetEndpoint("http");
var minioHttpEndpoint = minio.GetEndpoint("http");
var minioPublicBaseUrl = builder.Configuration["Apis:AccessOrigins:Minio"];
if (string.IsNullOrWhiteSpace(minioPublicBaseUrl))
    minioPublicBaseUrl = $"http://localhost:{PlatformPorts.MinioApi}";

catalogApi
    .WaitFor(minio)
    .WithReference(minioHttpEndpoint)
    .WithEnvironment("ImageStorage__Bucket", "article-images")
    .WithEnvironment("ImageStorage__Endpoint", minioHttpEndpoint)
    .WithEnvironment("ImageStorage__PublicBaseUrl", minioPublicBaseUrl.TrimEnd('/'))
    .WithEnvironment("ImageStorage__AccessKey", minioUser)
    .WithEnvironment("ImageStorage__SecretKey", minioPassword)
    .WithEnvironment("ImageStorage__AllowPublicRead", "true");

var apisByServiceName = new Dictionary<string, IResourceBuilder<ProjectResource>>(StringComparer.Ordinal)
{
    [ApiDirectory.Identity.ServiceName] = identityApi,
    [ApiDirectory.Catalog.ServiceName] = catalogApi,
    [ApiDirectory.Customers.ServiceName] = customersApi,
    [ApiDirectory.Sales.ServiceName] = salesApi
};

AddWebApp("storefront-web", "../Matterway.Storefront.Web", PlatformPorts.StorefrontWeb, [catalogApi, salesApi]);
AddWebApp("dashboard-web", "../Matterway.Dashboard.Web", PlatformPorts.DashboardWeb,
    [identityApi, catalogApi, customersApi, salesApi]);

if (builder.Environment.IsDevelopment())
    ScalarComposition.AddScalarApiReference(builder, apisByServiceName);

ApiEnvironmentComposition.ConfigureApiEnvironment(
    apisByServiceName.Values,
    identityApiHttp,
    jwtSigningKey,
    builder.Configuration);

builder.Build().Run();

IResourceBuilder<ProjectResource> AddApi<TProject>(
    ApiDefinition apiDefinition,
    int hostPort,
    Func<IResourceBuilder<ProjectResource>, IResourceBuilder<ProjectResource>> configure)
    where TProject : IProjectMetadata, new()
{
    var api = builder.AddProject<TProject>($"{apiDefinition.ServiceName}-api")
        .WithExternalHttpEndpoints()
        .PublishAsDockerComposeService((_, service) =>
        {
            service.Restart = "unless-stopped";
            service.Ports = [$"{hostPort}:8080"];
        });

    return configure(api);
}

void AddWebApp(
    string serviceName,
    string relativePath,
    int hostPort,
    IReadOnlyList<IResourceBuilder<ProjectResource>> dependencies)
{
    const string httpEndpoint = "http";

    if (builder.ExecutionContext.IsPublishMode)
    {
        var published = builder.AddDockerfile(serviceName, relativePath)
            .WithEnvironment("PORT", hostPort.ToString())
            .WithHttpEndpoint(hostPort, hostPort, httpEndpoint);

        foreach (var dependency in dependencies)
            published = published.WaitFor(dependency);

        var configured = ConfigureWebEnvironment(published);
        if (string.Equals(serviceName, "storefront-web", StringComparison.Ordinal))
            configured = configured.WithEnvironment("NUXT_STRIPE_SECRET_KEY", stripeSecretKey);

        configured.PublishAsDockerComposeService((_, service) =>
        {
            service.Restart = "unless-stopped";
            service.Ports = [$"{hostPort}:{hostPort}"];
        });

        return;
    }

    var development = builder.AddViteApp(serviceName, relativePath)
        .WithEnvironment("PORT", hostPort.ToString())
        .WithEndpoint(httpEndpoint, endpoint =>
        {
            endpoint.Port = hostPort;
            endpoint.IsProxied = false;
        })
        .WithExternalHttpEndpoints()
        .PublishAsDockerFile();

    foreach (var dependency in dependencies)
        development = development.WaitFor(dependency);

    var configuredDevelopment = ConfigureWebEnvironment(development);
    if (string.Equals(serviceName, "storefront-web", StringComparison.Ordinal))
        configuredDevelopment = configuredDevelopment.WithEnvironment("NUXT_STRIPE_SECRET_KEY", stripeSecretKey);

    configuredDevelopment.PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = [$"{hostPort}:{hostPort}"];
    });
}

IResourceBuilder<T> ConfigureWebEnvironment<T>(IResourceBuilder<T> resource)
    where T : IResource, IResourceWithEnvironment
{
    var endpointVariables = new (string Name, EndpointReference Value)[]
    {
        ("NUXT_SERVER_IDENTITY_API_BASE_URL", identityApiHttp),
        ("NUXT_SERVER_CATALOG_API_BASE_URL", catalogApiHttp),
        ("NUXT_SERVER_CUSTOMERS_API_BASE_URL", customersApiHttp),
        ("NUXT_SERVER_SALES_API_BASE_URL", salesApiHttp),
        ("IDENTITY_API_BASE_URL", identityApiHttp),
        ("CATALOG_API_BASE_URL", catalogApiHttp),
        ("CUSTOMERS_API_BASE_URL", customersApiHttp),
        ("SALES_API_BASE_URL", salesApiHttp)
    };

    foreach (var (name, value) in endpointVariables)
        resource = resource.WithEnvironment(name, value);

    var apiAccessOriginVariables = new (string Name, string ConfigKey)[]
    {
        ("NUXT_PUBLIC_IDENTITY_API_BASE_URL", "Apis:AccessOrigins:Identity"),
        ("NUXT_PUBLIC_CATALOG_API_BASE_URL", "Apis:AccessOrigins:Catalog"),
        ("NUXT_PUBLIC_CUSTOMERS_API_BASE_URL", "Apis:AccessOrigins:Customers"),
        ("NUXT_PUBLIC_SALES_API_BASE_URL", "Apis:AccessOrigins:Sales")
    };

    foreach (var (name, configKey) in apiAccessOriginVariables)
    {
        var value = builder.Configuration[configKey];
        if (!string.IsNullOrWhiteSpace(value))
            resource = resource.WithEnvironment(name, value.TrimEnd('/'));
    }

    return resource;
}