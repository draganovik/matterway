using Matterway.AppHost.Composition;
using Matterway.ServiceDefaults;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var jwtSigningKey = builder.AddParameter("JwtSigningKey", true);
var postgresPassword = builder.AddParameter("PostgresPassword", true);
var minioUser = builder.AddParameter("MinioRootUser");
var minioPassword = builder.AddParameter("MinioRootPassword", true);

// Configure shared local infrastructure first (dashboard, data stores, object storage).
ConfigureDashboard(builder);

var postgres = builder.AddPostgres("postgres")
    .WithImageTag("18")
    .WithHostPort(15432)
    .WithPassword(postgresPassword)
    .WithVolume("matterway-postgres-data", "/var/lib/postgresql")
    .WithBindMount("../../data", "/seed-data", true)
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["15432:5432"];
    });

var catalogDb = postgres.AddDatabase("CatalogDb");
var customersDb = postgres.AddDatabase("CustomersDb");
var identityDb = postgres.AddDatabase("IdentityDb");
var salesDb = postgres.AddDatabase("SalesDb");

var minio = builder.AddContainer("minio", "minio/minio:latest")
    .WithVolume("matterway-minio-data", "/data")
    .WithEnvironment("MINIO_ROOT_USER", minioUser)
    .WithEnvironment("MINIO_ROOT_PASSWORD", minioPassword)
    .WithArgs("server", "/data", "--console-address", ":9001")
    .WithHttpEndpoint(19000, 9000, "http")
    .WithHttpEndpoint(19001, 9001, "console")
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Command = ["server", "/data", "--console-address", ":9001"];
        service.Ports = ["19000:9000", "19001:9001"];
    });

// Register APIs in dependency order so references are explicit and startup waits are correct.
var identityApi = ConfigureApiResource(
    builder.AddProject<Matterway_Identity_Api>(ToApiResourceName(ApiDirectory.Identity))
        .WithReference(identityDb),
    2003);

var catalogApi = ConfigureApiResource(
    builder.AddProject<Matterway_Catalog_Api>(ToApiResourceName(ApiDirectory.Catalog))
        .WithReference(catalogDb),
    2001);

catalogApi
    .WaitFor(minio)
    .WithReference(minio.GetEndpoint("http"))
    .WithEnvironment("ImageStorage__Bucket", "article-images")
    .WithEnvironment("ImageStorage__Endpoint", minio.GetEndpoint("http"))
    .WithEnvironment("ImageStorage__PublicBaseUrl", minio.GetEndpoint("http"))
    .WithEnvironment("ImageStorage__AccessKey", minioUser)
    .WithEnvironment("ImageStorage__SecretKey", minioPassword)
    .WithEnvironment("ImageStorage__AllowPublicRead", "true");

var customersApi = ConfigureApiResource(
    builder.AddProject<Matterway_Customers_Api>(ToApiResourceName(ApiDirectory.Customers))
        .WithReference(customersDb)
        .WithReference(identityApi.GetEndpoint("http"))
        .WithReference(catalogApi.GetEndpoint("http")),
    2002);

var salesApi = ConfigureApiResource(
    builder.AddProject<Matterway_Sales_Api>(ToApiResourceName(ApiDirectory.Sales))
        .WithReference(salesDb)
        .WithReference(customersApi.GetEndpoint("http")),
    2005);

var apisByServiceName = new Dictionary<string, IResourceBuilder<ProjectResource>>(StringComparer.Ordinal)
{
    [ApiDirectory.Identity.ServiceName] = identityApi,
    [ApiDirectory.Catalog.ServiceName] = catalogApi,
    [ApiDirectory.Customers.ServiceName] = customersApi,
    [ApiDirectory.Sales.ServiceName] = salesApi
};

// Compose frontend apps once all API endpoints are known.
var web = WebComposition.AddWebApps(builder, identityApi, catalogApi, customersApi, salesApi);

// Apply cross-cutting API wiring (docs + auth/cors environment) in one place.
ScalarComposition.AddScalarApiReference(builder, apisByServiceName);
ApiEnvironmentComposition.ConfigureApiEnvironment(
    apisByServiceName.Values,
    identityApi.GetEndpoint("http"),
    web,
    jwtSigningKey);

builder.Build().Run();

static void ConfigureDashboard(IDistributedApplicationBuilder builder)
{
    builder.AddDockerComposeEnvironment("matterway-platform").WithDashboard(options =>
    {
        options.WithHostPort(18888);
        options.WithContainerName("aspire-dashboard");
    }).ConfigureComposeFile(compose => { compose.Name = "matterway-erp-stack"; });
}

static string ToApiResourceName(ApiDefinition apiDefinition)
{
    return $"{apiDefinition.ServiceName}-api";
}

static IResourceBuilder<ProjectResource> ConfigureApiResource(
    IResourceBuilder<ProjectResource> resourceBuilder,
    int hostPort)
{
    return resourceBuilder
        .WithExternalHttpEndpoints()
        .PublishAsDockerComposeService((_, service) =>
        {
            service.Restart = "unless-stopped";
            service.Ports = [$"{hostPort}:8080"];
        });
}