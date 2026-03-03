using Matterway.AppHost.Composition;
using Matterway.ServiceDefaults;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var jwtSigningKey = builder.AddParameter("JwtSigningKey", true);
var postgresPassword = builder.AddParameter("PostgresPassword", true);
var minioUser = builder.AddParameter("MinioRootUser");
var minioPassword = builder.AddParameter("MinioRootPassword", true);
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
    .WithHttpEndpoint(19000, 9000, "http")
    .WithHttpEndpoint(19001, 9001, "console")
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Command = ["server", "/data", "--console-address", ":9001"];
        service.Ports = ["19000:9000", "19001:9001"];
    });

var identityApi = AddApi<Matterway_Identity_Api>(builder, ApiDirectory.Identity, 2003,
    resource => resource.WithReference(databases.Identity));
var catalogApi = AddApi<Matterway_Catalog_Api>(builder, ApiDirectory.Catalog, 2001,
    resource => resource.WithReference(databases.Catalog));
var customersApi = AddApi<Matterway_Customers_Api>(builder, ApiDirectory.Customers, 2002, resource => resource
    .WithReference(databases.Customers)
    .WithReference(identityApi.GetEndpoint("http"))
    .WithReference(catalogApi.GetEndpoint("http")));
var salesApi = AddApi<Matterway_Sales_Api>(builder, ApiDirectory.Sales, 2005, resource => resource
    .WithReference(databases.Sales)
    .WithReference(customersApi.GetEndpoint("http")));

var minioHttpEndpoint = minio.GetEndpoint("http");
catalogApi
    .WaitFor(minio)
    .WithReference(minioHttpEndpoint)
    .WithEnvironment("ImageStorage__Bucket", "article-images")
    .WithEnvironment("ImageStorage__Endpoint", minioHttpEndpoint)
    .WithEnvironment("ImageStorage__PublicBaseUrl", minioHttpEndpoint)
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
var web = WebComposition.AddWebApps(builder, identityApi, catalogApi, customersApi, salesApi);
if (builder.Environment.IsDevelopment())
    ScalarComposition.AddScalarApiReference(builder, apisByServiceName);
ApiEnvironmentComposition.ConfigureApiEnvironment(apisByServiceName.Values, identityApi.GetEndpoint("http"), web,
    jwtSigningKey);
builder.Build().Run();

static IResourceBuilder<ProjectResource> AddApi<TProject>(
    IDistributedApplicationBuilder builder,
    ApiDefinition apiDefinition,
    int hostPort,
    Func<IResourceBuilder<ProjectResource>, IResourceBuilder<ProjectResource>> configure)
    where TProject : IProjectMetadata, new()
{
    return ConfigureApiResource(configure(builder.AddProject<TProject>($"{apiDefinition.ServiceName}-api")), hostPort);
}

static void ConfigureDashboard(IDistributedApplicationBuilder builder)
{
    builder.AddDockerComposeEnvironment("matterway-platform").WithDashboard(options =>
    {
        options.WithHostPort(18888);
        options.WithContainerName("aspire-dashboard");
    }).ConfigureComposeFile(compose => { compose.Name = "matterway-erp-stack"; });
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