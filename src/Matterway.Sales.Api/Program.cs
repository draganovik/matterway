using Matterway.Sales.Api.Infrastructure.Brokers.Customers;
using Matterway.Sales.Api.Infrastructure.Persistence;
using Matterway.ServiceDefaults;
using Matterway.ServiceDefaults.Extensions;

var apiContract = ApiDirectory.Sales;

var builder = BuilderBootstrap.CreateBuilder(args);

builder
    .ConfigureApiFoundation(apiContract)
    .ConfigureAuthentication()
    .ConfigureCustomersIntegration()
    .ConfigurePersistence()
    .ConfigureEndpoints();

var app = builder.Build();

app.UseApiFoundation();
app.ApplyApiContract(apiContract);

app.Run();