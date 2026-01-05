using FastEndpoints;
using Scalar.AspNetCore;

namespace Matterway.Customers.Api.Application;

public static class EndpointRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureFastEndpoints()
        {
            builder.Services.AddFastEndpoints();

            return builder;
        }
    }

    extension(WebApplication app)
    {
        public WebApplication UseFastEndpointsWithDefaults()
        {
            app.UseFastEndpoints(config =>
            {
                config.Endpoints.RoutePrefix = "api";
                config.Versioning.Prefix = "v";
                config.Versioning.PrependToRoute = true;
                config.Versioning.DefaultVersion = 1;
            });

            return app;
        }

        public WebApplication ApplyScalar()
        {
            app.MapScalarApiReference("/", options =>
            {
                options.WithOpenApiRoutePattern("/openapi/{documentName}.yaml");
                options.WithTitle("Matterway Customers API");
            });

            return app;
        }
    }
}