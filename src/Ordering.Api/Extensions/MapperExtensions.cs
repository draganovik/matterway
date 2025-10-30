using Matterway.Common.Mapping;

namespace Ordering.Api.Extensions;

public static class MapperExtensions
{
    public static IServiceCollection ConfigureMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<ValidationProfile>(), AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}