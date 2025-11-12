using Projects;

var builder = DistributedApplication.CreateBuilder(args);

const string productImagesBucket = "product-images";
var jwtSigningKey = builder.AddParameter("JwtSigningKey", secret: true);
var postgresPassword = builder.AddParameter("PostgresPassword", secret: true);
var minioUser = builder.AddParameter("MinioRootUser");
var minioPassword = builder.AddParameter("MinioRootPassword", secret: true);

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
var paymentsDb = postgres.AddDatabase("PaymentsDb");
var orderingDb = postgres.AddDatabase("OrderingDb");

// Setup MinIO for product image storage
var minio = builder.AddContainer("minio", "minio/minio:latest")
    .WithVolume("matterway-minio-data", "/data")
    .WithEnvironment("MINIO_ROOT_USER", minioUser)
    .WithEnvironment("MINIO_ROOT_PASSWORD", minioPassword)
    .WithArgs("server", "/data", "--console-address", ":9001")
    .WithHttpEndpoint(port: 19000, targetPort: 9000, name: "http")
    .WithHttpEndpoint(port: 19001, targetPort: 9001, name: "console")
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
    .WaitFor(minio)
    .WithReference(catalogDb)
    .WithReference(identityApi.GetEndpoint("http"))
    .WithReference(minio.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2001:8080"];
    });

catalogApi
    .WithEnvironment("ImageStorage__Bucket", productImagesBucket)
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

// Setup Inventory API
var inventoryApi = builder.AddProject<Matterway_Inventory_Api>("inventory-api")
    .WithReference(identityApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2004:8080"];
    });

// Setup Ordering API
var orderingApi = builder.AddProject<Matterway_Ordering_Api>("ordering-api")
    .WithReference(orderingDb)
    .WithReference(identityApi.GetEndpoint("http"))
    .WithReference(catalogApi.GetEndpoint("http"))
    .WithReference(customersApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2005:8080"];
    });

// Setup Payments API
var paymentsApi = builder.AddProject<Matterway_Payments_Api>("payments-api")
    .WithReference(paymentsDb)
    .WithReference(identityApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2006:8080"];
    });

// Setup Storefront Web Application
var storefront = builder.AddViteApp("storefront-web", "../Matterway.Storefront.Web")
    .WaitFor(catalogApi)
    .WithEnvironment("NUXT_SERVER_ORDERING_API_BASE_URL", orderingApi.GetEndpoint("http"))
    .WithEnvironment("NUXT_SERVER_PAYMENTS_API_BASE_URL", paymentsApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["3001:8000"];
    });

ConfigureApiJwtSettings(catalogApi);
ConfigureApiJwtSettings(customersApi);
ConfigureApiJwtSettings(identityApi);
ConfigureApiJwtSettings(inventoryApi);
ConfigureApiJwtSettings(orderingApi);
ConfigureApiJwtSettings(paymentsApi);

ConfigureApiCorsOrigins(catalogApi);
ConfigureApiCorsOrigins(customersApi);
ConfigureApiCorsOrigins(identityApi);
ConfigureApiCorsOrigins(inventoryApi);
ConfigureApiCorsOrigins(orderingApi);
ConfigureApiCorsOrigins(paymentsApi);

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