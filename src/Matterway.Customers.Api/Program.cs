using Matterway.Customers.Api.Infrastructure.Brokers.Catalog;
using Matterway.Customers.Api.Infrastructure.Brokers.Identity;
using Matterway.Customers.Api.Infrastructure.Persistence;
using Matterway.ServiceDefaults;
using Matterway.ServiceDefaults.Extensions;

var apiContract = ApiDirectory.Customers;

var builder = BuilderBootstrap.CreateBuilder(args);

builder
    .ConfigureApi(apiContract)
    .ConfigureAuthentication()
    .ConfigureApiHttpClient<ICatalogClient, HttpCatalogClient>(ApiDirectory.Catalog)
    .ConfigureApiHttpClient<IIdentityClient, HttpIdentityClient>(ApiDirectory.Identity)
    .ConfigurePersistence()
    .ConfigureEndpoints();

var app = builder.Build();

app.UseApiFoundation();
app.ApplyApiContract(apiContract);

app.Run();