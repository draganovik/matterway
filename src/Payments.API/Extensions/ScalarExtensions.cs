using Scalar.AspNetCore;

namespace Payments.API.Extensions;

public static class ScalarExtensions
{
    public static WebApplication ApplyScalar(this WebApplication app)
    {
        app.MapScalarApiReference("/", options =>
        {
            options.WithOpenApiRoutePattern("/openapi/{documentName}.yaml");
            options.WithTitle("Matterway Payments API");
        });

        return app;
    }
}