using Common.Infrastructure.Mapping;

namespace Customers.Api.Extensions;

public static class MapperExtensions
{
    public static IServiceCollection ConfigureMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<ValidationProfile>(), AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}