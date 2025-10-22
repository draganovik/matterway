using Scalar.AspNetCore;

namespace Identity.API.Extensions;

public static class ScalarExtensions
{
    public static WebApplication ApplyScalar(this WebApplication app)
    {
        app.MapScalarApiReference("/", options =>
        {
            options.WithOpenApiRoutePattern("/openapi/{documentName}.yaml");
            options.WithTitle("Matterway Identity API");
        });

        return app;
    }
}