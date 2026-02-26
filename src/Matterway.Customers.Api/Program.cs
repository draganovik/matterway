using Matterway.Customers.Api.Infrastructure.Brokers.Catalog;
using Matterway.Customers.Api.Infrastructure.Brokers.Identity;
using Matterway.Customers.Api.Infrastructure.Persistence;

var apiContract = ApiContracts.Customers;

var builder = ApiTemplate.CreateApiBuilder(args);

builder
    .ConfigureApiFoundation(apiContract)
    .ConfigureAuthentication(new ApiAuthenticationFeatureOptions())
    .ConfigureCatalogIntegration()
    .ConfigureIdentityIntegration()
    .ConfigurePersistence()
    .ConfigureFeatures(new ApiFeatureDiscoveryOptions());

var app = builder.Build();

app.UseApiFoundation();
app.ApplyApiContract(apiContract);

app.Run();