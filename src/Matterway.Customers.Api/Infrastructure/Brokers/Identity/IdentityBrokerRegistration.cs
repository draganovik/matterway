using Matterway.ServiceDefaults;

namespace Matterway.Customers.Api.Infrastructure.Brokers.Identity;

public static class IdentityBrokerRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureIdentityIntegration()
        {
            builder.Services.AddHttpClient<IIdentityClient, HttpIdentityClient>((sp, client) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = configuration.ResolveServiceUri("identity-api", "Services:Identity:Url");
            });

            return builder;
        }
    }
}