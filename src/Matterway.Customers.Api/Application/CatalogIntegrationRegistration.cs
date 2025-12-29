using Matterway.Customers.Api.Infrastructure.Brokers.Catalog;
using Matterway.ServiceDefaults;

namespace Matterway.Customers.Api.Application;

public static class CatalogIntegrationRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureCatalogIntegration()
        {
            builder.Services.AddHttpClient<ICatalogClient, HttpCatalogClient>((sp, client) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = configuration.ResolveServiceUri("catalog-api", "Services:Catalog:Url");
            });

            return builder;
        }
    }
}