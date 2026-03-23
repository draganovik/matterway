using Matterway.ServiceDefaults;
using Matterway.ServiceDefaults.Bootstraps;
using Scalar.Aspire;

namespace Matterway.AppHost.ApiDocumentation;

internal static class ScalarApiRegistration
{
    public static void AddApiReferences(
        IDistributedApplicationBuilder builder,
        IReadOnlyDictionary<string, IResourceBuilder<ProjectResource>> apisByServiceName)
    {
        var scalarApiReference = builder.AddScalarApiReference();

        foreach (var apiDefinition in ApiDirectory.All)
        {
            if (!apisByServiceName.TryGetValue(apiDefinition.ServiceName, out var resource))
                throw new KeyNotFoundException(
                    $"No API resource registered for service '{apiDefinition.ServiceName}'. " +
                    "Ensure it is registered in the AppHost configuration.");

            scalarApiReference.WithApiReference(resource, options =>
            {
                foreach (var documentName in GetDocumentNames(apiDefinition))
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