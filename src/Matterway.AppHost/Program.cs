using Projects;

var builder = DistributedApplication.CreateBuilder(args);

const string articleImagesBucket = "article-images";
var jwtSigningKey = builder.AddParameter("JwtSigningKey", true);
var postgresPassword = builder.AddParameter("PostgresPassword", true);
var minioUser = builder.AddParameter("MinioRootUser");
var minioPassword = builder.AddParameter("MinioRootPassword", true);

// Setup Docker Compose environment for dashboard
builder.AddDockerComposeEnvironment("matterway-platform").WithDashboard(options =>
{
    options.WithHostPort(18888);
    options.WithContainerName("aspire-dashboard");
}).ConfigureComposeFile(compose => { compose.Name = "matterway-erp-stack"; });

// Setup PostgreSQL container with databases
var postgres = builder.AddPostgres("postgres")
    .WithImageTag("18")
    .WithHostPort(15432)
    .WithPassword(postgresPassword)
    .WithVolume("matterway-postgres-data", "/var/lib/postgresql")
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["15432:5432"];
    });

var catalogDb = postgres.AddDatabase("CatalogDb");
var customersDb = postgres.AddDatabase("CustomersDb");
var identityDb = postgres.AddDatabase("IdentityDb");
var salesDb = postgres.AddDatabase("SalesDb");

// Setup MinIO for article image storage
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

// Setup Identity API
var identityApi = builder.AddProject<Matterway_Identity_Api>("identity-api")
    .WithReference(identityDb)
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2003:8080"];
    });

// Setup Catalog API
var catalogApi = builder.AddProject<Matterway_Catalog_Api>("catalog-api")
    .WithReference(catalogDb)
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2001:8080"];
    });

catalogApi
    .WaitFor(minio)
    .WithReference(minio.GetEndpoint("http"))
    .WithEnvironment("ImageStorage__Bucket", articleImagesBucket)
    .WithEnvironment("ImageStorage__Endpoint", minio.GetEndpoint("http"))
    .WithEnvironment("ImageStorage__PublicBaseUrl", minio.GetEndpoint("http"))
    .WithEnvironment("ImageStorage__AccessKey", minioUser)
    .WithEnvironment("ImageStorage__SecretKey", minioPassword)
    .WithEnvironment("ImageStorage__AllowPublicRead", "true");

// Setup Customers API
var customersApi = builder.AddProject<Matterway_Customers_Api>("customers-api")
    .WithReference(customersDb)
    .WithReference(identityApi.GetEndpoint("http"))
    .WithReference(catalogApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2002:8080"];
    });

// Setup Sales API
var salesApi = builder.AddProject<Matterway_Sales_Api>("sales-api")
    .WithReference(salesDb)
    .WithReference(customersApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2005:8080"];
    });

// Setup Storefront Web Application
var storefront = builder.AddViteApp("storefront-web", "../Matterway.Storefront.Web")
    .WaitFor(catalogApi)
    .WithEnvironment("NUXT_SERVER_SALES_API_BASE_URL", salesApi.GetEndpoint("http"))
    .WithEndpoint("http", e =>
    {
        e.TargetPort = 3000;
        e.Port = 3001;
    })
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["3001:3000"];
    });

ConfigureApiJwtSettings(catalogApi);
ConfigureApiJwtSettings(customersApi);
ConfigureApiJwtSettings(identityApi);
ConfigureApiJwtSettings(salesApi);

ConfigureApiCorsOrigins(catalogApi);
ConfigureApiCorsOrigins(customersApi);
ConfigureApiCorsOrigins(identityApi);
ConfigureApiCorsOrigins(salesApi);

// Run the application
builder.Build().Run();

return;

void ConfigureApiCorsOrigins(IResourceBuilder<ProjectResource> resource)
{
    resource.WithEnvironment("Cors__AllowedOrigins__0", storefront.GetEndpoint("http"));
}

void ConfigureApiJwtSettings(IResourceBuilder<ProjectResource> resource)
{
    resource
        .WithEnvironment("Jwt__Key", jwtSigningKey)
        .WithEnvironment("Jwt__Issuer", identityApi.GetEndpoint("http"))
        .WithEnvironment("Jwt__Audience", identityApi.GetEndpoint("http"));
}