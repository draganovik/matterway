using Matterway.Common.Extensions;
using Matterway.Common.Services.Brokers;

namespace Matterway.Catalog.Api.Extensions;

public static class ProxyServiceExtensions
{
    public static void ConfigureProxyServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IIdentityServiceBroker, IdentityServiceBroker>((sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = configuration.ResolveServiceUri("identity-api", "Services:Identity:Url");
        });
    }
}