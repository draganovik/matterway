using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("matterway-platform").WithDashboard(options =>
{
    options.WithHostPort(18888);
    options.WithContainerName("aspire-dashboard");
}).ConfigureComposeFile(compose => { compose.Name = "matterway-erp-stack"; });

var postgresPassword = builder.AddParameter("PostgresPassword", secret: true);
var jwtSigningKey = builder.AddParameter("JwtSigningKey", secret: true);


// Setup PostgreSQL container with databases
var postgres = builder.AddPostgres("postgres")
    .WithHostPort(15432)
    .WithPassword(postgresPassword)
    .WithDataVolume()
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

// Setup Identity API
var identityApi = builder.AddProject<Matterway_Identity_Api>("identity-api")
    .WithReference(identityDb)
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2003:8080"];
    });

var identityEndpoint = identityApi.GetEndpoint("http");

identityApi
    .WithEnvironment("Jwt__Key", jwtSigningKey)
    .WithEnvironment("Jwt__Issuer", identityEndpoint)
    .WithEnvironment("Jwt__Audience", identityEndpoint);

// Setup Catalog API
var catalogApi = builder.AddProject<Matterway_Catalog_Api>("catalog-api")
    .WithReference(catalogDb)
    .WithReference(identityApi)
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2001:8080"];
    });

catalogApi
    .WithEnvironment("Jwt__Key", jwtSigningKey)
    .WithEnvironment("Jwt__Issuer", identityEndpoint)
    .WithEnvironment("Jwt__Audience", identityEndpoint);

// Setup Customers API
var customersApi = builder.AddProject<Matterway_Customers_Api>("customers-api")
    .WithReference(customersDb)
    .WithReference(identityApi)
    .WithReference(catalogApi)
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2002:8080"];
    });

customersApi
    .WithEnvironment("Jwt__Key", jwtSigningKey)
    .WithEnvironment("Jwt__Issuer", identityEndpoint)
    .WithEnvironment("Jwt__Audience", identityEndpoint);

// Setup Inventory API
var inventoryApi = builder.AddProject<Matterway_Inventory_Api>("inventory-api")
    .WithReference(identityApi)
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2004:8080"];
    });

inventoryApi
    .WithEnvironment("Jwt__Key", jwtSigningKey)
    .WithEnvironment("Jwt__Issuer", identityEndpoint)
    .WithEnvironment("Jwt__Audience", identityEndpoint);

// Setup Ordering API
var orderingApi = builder.AddProject<Matterway_Ordering_Api>("ordering-api")
    .WithReference(orderingDb)
    .WithReference(identityApi)
    .WithReference(catalogApi)
    .WithReference(customersApi)
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2005:8080"];
    });

orderingApi
    .WithEnvironment("Jwt__Key", jwtSigningKey)
    .WithEnvironment("Jwt__Issuer", identityEndpoint)
    .WithEnvironment("Jwt__Audience", identityEndpoint);

// Setup Payments API
var paymentsApi = builder.AddProject<Matterway_Payments_Api>("payments-api")
    .WithReference(paymentsDb)
    .WithReference(identityApi)
    .WithExternalHttpEndpoints()
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Restart = "unless-stopped";
        service.Ports = ["2006:8080"];
    });

paymentsApi
    .WithEnvironment("Jwt__Key", jwtSigningKey)
    .WithEnvironment("Jwt__Issuer", identityEndpoint)
    .WithEnvironment("Jwt__Audience", identityEndpoint);

// Setup Storefront Web Application
var storefront = builder.AddNpmApp("storefront-web", "../Matterway.Storefront.Web")
    // server-only URLS, called from server-side code (inside the docker network)
    .WithEnvironment("NUXT_SERVER_ORDERING_API_BASE_URL", orderingApi.GetEndpoint("http"))
    .WithEnvironment("NUXT_SERVER_PAYMENTS_API_BASE_URL", paymentsApi.GetEndpoint("http"))
    .WaitFor(catalogApi)
    .WithHttpEndpoint(port: 3001, targetPort: 3000, name: "http")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile()
    .PublishAsDockerComposeService((_, service) => { service.Restart = "unless-stopped"; });

// Reference storefront in apis for CORS setup
catalogApi.WithReference(storefront);
customersApi.WithReference(storefront);
identityApi.WithReference(storefront);
orderingApi.WithReference(storefront);
paymentsApi.WithReference(storefront);
inventoryApi.WithReference(storefront);

// Run the application
builder.Build().Run();