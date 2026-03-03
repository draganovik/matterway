using Matterway.ServiceDefaults;

namespace Matterway.Customers.Api.Infrastructure.Brokers.Catalog;

public static class CatalogBrokerRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureCatalogIntegration()
        {
            builder.Services.AddHttpClient<ICatalogClient, HttpCatalogClient>((sp, client) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = configuration.ResolveServiceUri("catalog-api", "Apis:AccessOrigins:Catalog");
            });

            return builder;
        }
    }
}