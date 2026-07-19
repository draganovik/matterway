using Matterway.ServiceDefaults;
using Matterway.ServiceDefaults.Bootstraps;
using Scalar.Aspire;

namespace Matterway.AppHost.ApiDocumentation;

internal static class ScalarApiRegistration
{
    public static void AddApiReferences(
        IDistributedApplicationBuilder builder,
        int port,
        IEnumerable<(ApiDefinition Definition, IResourceBuilder<ProjectResource> Resource)> apiResources)
    {
        var scalarApiReference = builder.AddScalarApiReference("scalar", port);

        foreach (var (apiDefinition, resource) in apiResources)
            scalarApiReference.WithApiReference(resource, "http", options =>
            {
                options
                    .AddDocuments(GetDocumentNames(apiDefinition))
                    .WithOpenApiRoutePattern(ApiDocumentationDefaults.OpenApiRoutePattern);
            });
    }

    private static IEnumerable<string> GetDocumentNames(ApiDefinition apiDefinition)
    {
        return ApiVersioningConventions.NormalizeSupportedVersions(apiDefinition.SupportedApiVersions)
            .Select(ApiVersioningConventions.ToDocumentName);
    }
}
