using Matterway.AppHost.Composition;
using Matterway.ServiceDefaults;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);
const string composeEnvironmentName = "matterway-platform";
var services = ServicesSettings.Bind(builder.Configuration);

var jwtSigningKey = builder.AddParameter("JwtSigningKey", true);
var systemAccessKey = builder.AddParameter("SystemAccessKey", true);
var postgresPassword = builder.AddParameter("PostgresPassword", true);
var minioUser = builder.AddParameter("MinioRootUser");
var minioPassword = builder.AddParameter("MinioRootPassword", true);
var stripeSecretKey = builder.AddParameter("StripeSecretKey", true);

builder.AddDockerComposeEnvironment(composeEnvironmentName)
    .WithDashboard(dashboard =>
    {
        dashboard.WithHostPort(services.AspireDashboard.Port);
        dashboard.WithContainerName("aspire-dashboard");
    })
    .ConfigureComposeFile(compose => compose.Name = "matterway-platform");

var postgres = builder.AddPostgres("postgres")
    .WithImageTag("18")
    .WithHostPort(services.Postgres.Port)
    .WithPassword(postgresPassword)
    .WithVolume("matterway-postgres-data", "/var/lib/postgresql")
    .WithBindMount("../../data", "/seed-data", true)
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = [$"{services.Postgres.Port}:5432"];
    });

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
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = [$"{services.Minio.Port}:9000", $"{services.Minio.ConsolePort}:9001"];
    });

var dbMigrator = builder.AddProject<Matterway_Migrations>("db-migrator")
    .WithReference(databases.Catalog)
    .WithReference(databases.Customers)
    .WithReference(databases.Identity)
    .WithReference(databases.Sales)
    .WaitFor(postgres)
    .PublishAsDockerComposeService((_, service) => { service.Restart = "no"; });

var identityApi = AddApi<Matterway_Identity_Api>(
    ApiDirectory.Identity,
    services.Identity.Port,
    api => api.WithReference(databases.Identity));

var catalogApi = AddApi<Matterway_Catalog_Api>(
    ApiDirectory.Catalog,
    services.Catalog.Port,
    api => api.WithReference(databases.Catalog));

var customersApi = AddApi<Matterway_Customers_Api>(
    ApiDirectory.Customers,
    services.Customers.Port,
    api => api
        .WithReference(databases.Customers)
        .WithReference(identityApi.GetEndpoint("http"))
        .WithReference(catalogApi.GetEndpoint("http")));

var salesApi = AddApi<Matterway_Sales_Api>(
    ApiDirectory.Sales,
    services.Sales.Port,
    api => api.WithReference(databases.Sales)
        .WithReference(customersApi.GetEndpoint("http")));

var identityApiHttp = identityApi.GetEndpoint("http");
var catalogApiHttp = catalogApi.GetEndpoint("http");
var customersApiHttp = customersApi.GetEndpoint("http");
var salesApiHttp = salesApi.GetEndpoint("http");
var minioHttpEndpoint = minio.GetEndpoint("http");
var minioPublicBaseUrl = services.Minio.ToBaseUrl();
var identityPublicBaseUrl = services.Identity.ToBaseUrl();
var catalogPublicBaseUrl = services.Catalog.ToBaseUrl();
var customersPublicBaseUrl = services.Customers.ToBaseUrl();
var salesPublicBaseUrl = services.Sales.ToBaseUrl();

catalogApi
    .WaitFor(minio)
    .WithReference(minioHttpEndpoint)
    .WithEnvironment("ImageStorage__Bucket", "article-images")
    .WithEnvironment("ImageStorage__Endpoint", minioHttpEndpoint)
    .WithEnvironment("ImageStorage__PublicBaseUrl", minioPublicBaseUrl)
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

WebAppComposition.AddWebApp(
    builder,
    new WebAppCompositionOptions
    {
        ServiceName = "storefront-web",
        RelativePath = "../Matterway.Storefront.Web",
        HostPort = services.Storefront.Port,
        Dependencies = [catalogApi, salesApi],
        ConfigureEnvironment = environment =>
        {
            ConfigureCommonWebEnvironment(environment);
            ConfigureWebTelemetryEnvironment(environment, "storefront-web");
            environment.WithEnvironment("NUXT_SYSTEM_ACCESS_KEY", systemAccessKey);
            environment.WithEnvironment("NUXT_STRIPE_SECRET_KEY", stripeSecretKey);
        }
    });

WebAppComposition.AddWebApp(
    builder,
    new WebAppCompositionOptions
    {
        ServiceName = "dashboard-web",
        RelativePath = "../Matterway.Dashboard.Web",
        HostPort = services.Dashboard.Port,
        Dependencies = [identityApi, catalogApi, customersApi, salesApi],
        ConfigureEnvironment = ConfigureCommonWebEnvironment
    });

if (builder.Environment.IsDevelopment())
    ScalarComposition.AddScalarApiReference(builder, apisByServiceName);

ApiEnvironmentComposition.ConfigureApiEnvironment(
    apisByServiceName.Values,
    identityApiHttp,
    jwtSigningKey,
    systemAccessKey,
    builder.Configuration,
    [services.Storefront.ToBaseUrl(), services.Dashboard.ToBaseUrl()]);

builder.Build().Run();

IResourceBuilder<ProjectResource> AddApi<TProject>(
    ApiDefinition apiDefinition,
    int hostPort,
    Func<IResourceBuilder<ProjectResource>, IResourceBuilder<ProjectResource>> configure)
    where TProject : IProjectMetadata, new()
{
    var api = builder.AddProject<TProject>(apiDefinition.AspireServiceName)
        .WithExternalHttpEndpoints()
        .PublishAsDockerComposeService((_, service) =>
        {
            service.Restart = "unless-stopped";
            service.Ports = [$"{hostPort}:8080"];
        });

    return configure(api).WaitFor(dbMigrator);
}

void ConfigureWebTelemetryEnvironment(WebAppEnvironmentBuilder environment, string telemetryServiceName)
{
    static string? NormalizeHttpEndpoint(string? endpoint)
    {
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri))
            return null;
        if (endpointUri is not { Scheme: "http" or "https" })
            return null;

        return endpointUri.GetLeftPart(UriPartial.Path).TrimEnd('/');
    }

    static string? ToTraceEndpoint(string? endpoint)
    {
        var normalized = NormalizeHttpEndpoint(endpoint);
        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        return normalized.EndsWith("/v1/traces", StringComparison.OrdinalIgnoreCase)
            ? normalized
            : $"{normalized}/v1/traces";
    }

    static string? ToBaseEndpoint(string? endpoint)
    {
        var normalized = NormalizeHttpEndpoint(endpoint);
        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        return normalized.EndsWith("/v1/traces", StringComparison.OrdinalIgnoreCase)
            ? normalized[..^10].TrimEnd('/')
            : normalized;
    }

    var publishDefaultEndpoint = builder.ExecutionContext.IsPublishMode
        ? $"http://{composeEnvironmentName}-dashboard:{services.AspireDashboard.OtlpPort}"
        : null;

    var tracesEndpoint = ToTraceEndpoint(builder.Configuration["OTEL_EXPORTER_OTLP_TRACES_ENDPOINT"])
                         ?? ToTraceEndpoint(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"])
                         ?? ToTraceEndpoint(builder.Configuration["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"])
                         ?? ToTraceEndpoint(publishDefaultEndpoint);

    var baseEndpoint = ToBaseEndpoint(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"])
                       ?? ToBaseEndpoint(builder.Configuration["OTEL_EXPORTER_OTLP_TRACES_ENDPOINT"])
                       ?? ToBaseEndpoint(builder.Configuration["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"])
                       ?? ToBaseEndpoint(publishDefaultEndpoint);

    var protocol = builder.Configuration["OTEL_EXPORTER_OTLP_PROTOCOL"];
    if (string.IsNullOrWhiteSpace(protocol))
        protocol = "grpc";

    environment
        .WithEnvironment("OTEL_SERVICE_NAME", telemetryServiceName)
        .WithEnvironment("OTEL_RESOURCE_ATTRIBUTES", $"service.name={telemetryServiceName}");

    if (!string.IsNullOrWhiteSpace(tracesEndpoint))
        environment.WithTrimmedEnvironment("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT", tracesEndpoint);
    if (!string.IsNullOrWhiteSpace(baseEndpoint))
        environment.WithTrimmedEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", baseEndpoint);
    if (!string.IsNullOrWhiteSpace(tracesEndpoint) || !string.IsNullOrWhiteSpace(baseEndpoint))
        environment.WithEnvironment("OTEL_EXPORTER_OTLP_PROTOCOL", protocol);
}

void ConfigureCommonWebEnvironment(WebAppEnvironmentBuilder environment)
{
    environment
        .WithEnvironment("NUXT_SERVER_IDENTITY_API_BASE_URL", identityApiHttp)
        .WithEnvironment("NUXT_SERVER_CATALOG_API_BASE_URL", catalogApiHttp)
        .WithEnvironment("NUXT_SERVER_CUSTOMERS_API_BASE_URL", customersApiHttp)
        .WithEnvironment("NUXT_SERVER_SALES_API_BASE_URL", salesApiHttp)
        .WithEnvironment("IDENTITY_API_BASE_URL", identityApiHttp)
        .WithEnvironment("CATALOG_API_BASE_URL", catalogApiHttp)
        .WithEnvironment("CUSTOMERS_API_BASE_URL", customersApiHttp)
        .WithEnvironment("SALES_API_BASE_URL", salesApiHttp)
        .WithEnvironment("NUXT_PUBLIC_IDENTITY_API_BASE_URL", identityPublicBaseUrl)
        .WithEnvironment("NUXT_PUBLIC_CATALOG_API_BASE_URL", catalogPublicBaseUrl)
        .WithEnvironment("NUXT_PUBLIC_CUSTOMERS_API_BASE_URL", customersPublicBaseUrl)
        .WithEnvironment("NUXT_PUBLIC_SALES_API_BASE_URL", salesPublicBaseUrl);
}
