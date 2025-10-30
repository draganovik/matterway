using Matterway.Catalog.Api.Application.Repositories;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Catalog.Api.Infrastructure.Repositories;
using Matterway.Common.Extensions;
using Matterway.Common.Services.Brokers;
using Microsoft.EntityFrameworkCore;

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