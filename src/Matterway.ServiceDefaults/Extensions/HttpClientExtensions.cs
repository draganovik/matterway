using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Matterway.ServiceDefaults.Extensions;

public static class HttpClientExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureApiHttpClient<TClient, TImplementation>(ApiDefinition apiDefinition)
            where TClient : class
            where TImplementation : class, TClient
        {
            ArgumentNullException.ThrowIfNull(apiDefinition);

            builder.Services.AddHttpClient<TClient, TImplementation>((sp, client) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = configuration.ResolveServiceUri(apiDefinition);
            });

            return builder;
        }
    }
}