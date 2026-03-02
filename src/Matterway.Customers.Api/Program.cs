using Matterway.Customers.Api.Infrastructure.Brokers.Catalog;
using Matterway.Customers.Api.Infrastructure.Brokers.Identity;
using Matterway.Customers.Api.Infrastructure.Persistence;
using Matterway.ServiceDefaults;
using Matterway.ServiceDefaults.Extensions;

var apiContract = ApiDirectory.Customers;

var builder = BuilderBootstrap.CreateBuilder(args);

builder
    .ConfigureApiFoundation(apiContract)
    .ConfigureAuthentication()
    .ConfigureCatalogIntegration()
    .ConfigureIdentityIntegration()
    .ConfigurePersistence()
    .ConfigureEndpoints();

var app = builder.Build();

app.UseApiFoundation();
app.ApplyApiContract(apiContract);

app.Run();