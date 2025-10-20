using Scalar.AspNetCore;

namespace Catalog.API.Configurations;

public static class ScalarConfiguration
{
    public static WebApplication ApplyScalar(this WebApplication app)
    {
        app.MapScalarApiReference("/", options =>
        {
            options.WithOpenApiRoutePattern("/openapi/{documentName}.yaml");
            options.WithTitle("Matterway Catalog API");
        });

        return app;
    }
}