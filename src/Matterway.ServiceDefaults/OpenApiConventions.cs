using Asp.Versioning;
using Matterway.ServiceDefaults.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Matterway.ServiceDefaults;

internal static class OpenApiConventions
{
    public static bool ShouldIncludeInDocument(
        IEnumerable<object> endpointMetadata,
        ApiVersion documentVersion)
    {
        var versionMetadata = endpointMetadata
            .OfType<ApiVersionMetadata>()
            .LastOrDefault();
        if (versionMetadata is null)
            return false;

        return versionMetadata.IsMappedTo(documentVersion);
    }

    public static void SubstituteRouteVersion(OpenApiOptions options, ApiVersion documentVersion)
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            var majorVersion = documentVersion.MajorVersion!.Value;
            document.Info.Version = $"{majorVersion}.0.0";
            SubstituteRouteVersion(document, majorVersion.ToString());
            return Task.CompletedTask;
        });
    }

    public static void AddSecurity(OpenApiOptions options)
    {
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT authorization header using the Bearer scheme."
            };
            document.Components.SecuritySchemes["SystemAccessKey"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = SystemAccessKeyOptions.HeaderName,
                In = ParameterLocation.Header,
                Description = "Shared system access key header."
            };

            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, context, _) =>
        {
            var endpointMetadata = context.Description.ActionDescriptor.EndpointMetadata;
            var isAnonymous = endpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            var requiresSystemAccessKey =
                SystemAccessKeyEndpointExtensions.IsSystemAccessKeyRequired(endpointMetadata);

            if (isAnonymous && !requiresSystemAccessKey)
                return Task.CompletedTask;

            operation.Security ??= [];
            var requirement = new OpenApiSecurityRequirement();

            if (!isAnonymous)
            {
                var bearerReference = new OpenApiSecuritySchemeReference("Bearer");
                requirement.Add(bearerReference, []);
            }

            if (requiresSystemAccessKey)
            {
                var systemAccessKeyReference = new OpenApiSecuritySchemeReference("SystemAccessKey");
                requirement.Add(systemAccessKeyReference, []);
            }

            if (requirement.Count > 0)
                operation.Security.Add(requirement);

            return Task.CompletedTask;
        });
    }

    private static void SubstituteRouteVersion(OpenApiDocument document, string routeVersion)
    {
        if (document.Paths is null)
            return;

        var rewrittenPaths = new OpenApiPaths();

        foreach (var (path, pathItem) in document.Paths)
        {
            var rewrittenPath = path
                .Replace("v{version:apiVersion}", $"v{routeVersion}", StringComparison.OrdinalIgnoreCase)
                .Replace("v{version}", $"v{routeVersion}", StringComparison.OrdinalIgnoreCase);

            RemoveVersionParameter(pathItem.Parameters);

            if (pathItem.Operations is not null)
                foreach (var operation in pathItem.Operations.Values)
                    RemoveVersionParameter(operation.Parameters);

            rewrittenPaths[rewrittenPath] = pathItem;
        }

        document.Paths = rewrittenPaths;
    }

    private static void RemoveVersionParameter(IList<IOpenApiParameter>? parameters)
    {
        if (parameters is null)
            return;

        for (var index = parameters.Count - 1; index >= 0; index--)
        {
            var parameter = parameters[index];
            if (string.Equals(parameter.Name, "version", StringComparison.OrdinalIgnoreCase) &&
                parameter.In == ParameterLocation.Path)
                parameters.RemoveAt(index);
        }
    }
}