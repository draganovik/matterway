using Matterway.Common.Services.Brokers;
using Matterway.ServiceDefaults;

namespace Matterway.Catalog.Api.Extensions;

public static class ProxyServiceRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureProxyServices()
        {
            builder.Services.AddHttpClient<IIdentityServiceBroker, IdentityServiceBroker>((sp, client) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = configuration.ResolveServiceUri("identity-api", "Services:Identity:Url");
            });

            return builder;
        }
    }
}