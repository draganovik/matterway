using Matterway.ServiceDefaults;
using Matterway.ServiceDefaults.Bootstraps;
using Scalar.Aspire;

namespace Matterway.AppHost.Composition;

internal static class ScalarComposition
{
    public static void AddScalarApiReference(
        IDistributedApplicationBuilder builder,
        IReadOnlyDictionary<string, IResourceBuilder<ProjectResource>> apisByServiceName)
    {
        var scalarApiReference = builder.AddScalarApiReference();

        foreach (var apiContract in ApiDirectory.All)
        {
            var resource = apisByServiceName[apiContract.ServiceName];

            scalarApiReference.WithApiReference(resource, options =>
            {
                foreach (var documentName in GetDocumentNames(apiContract))
                    options.AddDocument(documentName, documentName);

                options.WithOpenApiRoutePattern(ApiDocumentationDefaults.OpenApiRoutePattern);
            });
        }
    }

    private static IEnumerable<string> GetDocumentNames(ApiDefinition apiDefinition)
    {
        return ApiVersioningConventions.NormalizeSupportedVersions(apiDefinition.SupportedApiVersions)
            .Select(ApiVersioningConventions.ToDocumentName);
    }
}