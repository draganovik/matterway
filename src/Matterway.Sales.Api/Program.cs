using Matterway.Sales.Api.Infrastructure.Brokers.Customers;
using Matterway.Sales.Api.Infrastructure.Persistence;

var apiContract = ApiContracts.Sales;

var builder = ApiTemplate.CreateApiBuilder(args);

builder
    .ConfigureApiFoundation(apiContract)
    .ConfigureAuthentication(new ApiAuthenticationFeatureOptions())
    .ConfigureCustomersIntegration()
    .ConfigurePersistence()
    .ConfigureFeatures(new ApiFeatureDiscoveryOptions());

var app = builder.Build();

app.UseApiFoundation();
app.ApplyApiContract(apiContract);

app.Run();