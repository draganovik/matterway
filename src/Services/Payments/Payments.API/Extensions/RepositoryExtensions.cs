using Payments.API.Features.Payments.Data;
using Shared.ServiceBrokers;

namespace Payments.API.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IIdentityServiceBroker, IdentityServiceBroker>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }
}