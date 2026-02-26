using Matterway.ServiceDefaults.Api;
using Matterway.ServiceDefaults.Versioning;
using Scalar.Aspire;

namespace Matterway.AppHost.Composition;

internal static class ScalarComposition
{
    public static void AddScalarApiReference(
        IDistributedApplicationBuilder builder,
        IReadOnlyDictionary<string, IResourceBuilder<ProjectResource>> apisByServiceName)
    {
        var scalarApiReference = builder.AddScalarApiReference();

        foreach (var apiContract in ApiContracts.All)
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

    private static IEnumerable<string> GetDocumentNames(ApiContract apiContract)
    {
        return ApiVersioningConventions.NormalizeSupportedVersions(apiContract.SupportedApiVersions)
            .Select(ApiVersioningConventions.ToDocumentName);
    }
}
