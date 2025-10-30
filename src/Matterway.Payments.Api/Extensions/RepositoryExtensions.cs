using Matterway.Common.Extensions;
using Matterway.Common.Services.Brokers;
using Matterway.Payments.Api.Features.Payments.Data;

namespace Matterway.Payments.Api.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddHttpClient<IIdentityServiceBroker, IdentityServiceBroker>((sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = configuration.ResolveServiceUri("identity-api", "Services:Identity:Url");
        });
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }
}