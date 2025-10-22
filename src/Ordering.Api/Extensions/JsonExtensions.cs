using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;

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