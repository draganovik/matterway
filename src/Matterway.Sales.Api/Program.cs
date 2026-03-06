using Matterway.Sales.Api.Infrastructure.Brokers.Customers;
using Matterway.Sales.Api.Infrastructure.Persistence;
using Matterway.ServiceDefaults;
using Matterway.ServiceDefaults.Extensions;

var apiContract = ApiDirectory.Sales;

var builder = BuilderBootstrap.CreateBuilder(args);

builder
    .ConfigureApi(apiContract)
    .ConfigureAuthentication()
    .ConfigureApiHttpClient<ICustomersClient, HttpCustomersClient>(ApiDirectory.Customers)
    .ConfigurePersistence()
    .ConfigureEndpoints();

var app = builder.Build();

app.UseApiFoundation();
app.ApplyApiContract(apiContract);

app.Run();