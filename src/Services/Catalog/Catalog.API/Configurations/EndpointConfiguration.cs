using Asp.Versioning;
using Shared.Extensions;

namespace Catalog.API.Configurations;

public static class EndpointConfiguration
{
    public static WebApplication ApplyEndpoints(this WebApplication app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var apiGroup = app.MapGroup("/api");
        var versionedGroup = apiGroup
            .MapGroup("/v{version:apiVersion}")
            .WithApiVersionSet(versionSet);

        app.MapEndpoints(versionedGroup);

        return app;
    }
}