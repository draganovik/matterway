using Payments.Api.Features.Payments.Data;
using Common.Infrastructure.Services.Brokers;

namespace Payments.Api.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IIdentityServiceBroker, IdentityServiceBroker>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }
}
