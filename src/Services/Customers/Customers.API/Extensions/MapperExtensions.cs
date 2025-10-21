using SharedProject.Profiles;

namespace Customers.API.Extensions;

public static class MapperExtensions
{
    public static IServiceCollection ConfigureMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<ValidationProfile>(), AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}