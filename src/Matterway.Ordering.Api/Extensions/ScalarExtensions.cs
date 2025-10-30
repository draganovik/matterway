using Scalar.AspNetCore;

namespace Matterway.Ordering.Api.Extensions;

public static class ScalarExtensions
{
    public static WebApplication ApplyScalar(this WebApplication app)
    {
        app.MapScalarApiReference("/", options =>
        {
            options.WithOpenApiRoutePattern("/openapi/{documentName}.yaml");
            options.WithTitle("Matterway Ordering API");
        });

        return app;
    }
}