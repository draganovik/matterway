using Matterway.ServiceDefaults;

namespace Matterway.Sales.Api.Providers.Brokers.Customers;

public static class CustomersBrokerRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureCustomersIntegration()
        {
            builder.Services.AddHttpClient<ICustomersClient, HttpCustomersClient>((sp, client) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = configuration.ResolveServiceUri("customers-api", "Services:Customers:Url");
            });

            return builder;
        }
    }
}