using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

namespace Ordering.Api.Extensions;

public static class JsonExtensions
{
    public static IServiceCollection ConfigureJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}