using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Matterway.ServiceDefaults.Versioning;

public static class ApiVersioningConventions
{
    public static ApiVersion[] NormalizeSupportedVersions(IReadOnlyCollection<ApiVersion> supportedApiVersions)
    {
        if (supportedApiVersions.Count == 0)
            throw new ArgumentException("At least one supported API version is required.",
                nameof(supportedApiVersions));

        return supportedApiVersions
            .Distinct()
            .OrderBy(version => version.MajorVersion ?? 0)
            .ThenBy(version => version.MinorVersion ?? 0)
            .ToArray();
    }

    public static string ToDocumentName(ApiVersion version)
    {
        var majorVersion = version.MajorVersion ?? 0;
        var minorVersion = version.MinorVersion ?? 0;

        return minorVersion > 0
            ? $"v{majorVersion}.{minorVersion}"
            : $"v{majorVersion}";
    }

    public static string[] ToDocumentNames(IReadOnlyCollection<ApiVersion> supportedApiVersions)
    {
        return NormalizeSupportedVersions(supportedApiVersions)
            .Select(ToDocumentName)
            .ToArray();
    }

    public static bool ShouldIncludeInDocument(ApiDescription description, ApiVersion documentVersion)
    {
        if (TryParseVersionFromRelativePath(description.RelativePath, out var pathVersion))
            return pathVersion == documentVersion;

        var versionMetadata = description.ActionDescriptor.EndpointMetadata
            .OfType<ApiVersionMetadata>()
            .LastOrDefault();
        if (versionMetadata is null)
            return false;

        var versionModel = versionMetadata.Map(ApiVersionMapping.Explicit);
        return versionModel.DeclaredApiVersions.Contains(documentVersion) ||
               versionModel.ImplementedApiVersions.Contains(documentVersion);
    }

    public static void AddBearerSecurity(OpenApiOptions options)
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

            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, context, _) =>
        {
            var isAnonymous = context.Description.ActionDescriptor
                .EndpointMetadata.OfType<AllowAnonymousAttribute>()
                .Any();
            if (isAnonymous) return Task.CompletedTask;

            operation.Security ??= [];
            var bearerReference = new OpenApiSecuritySchemeReference("Bearer");
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                { bearerReference, [] }
            });

            return Task.CompletedTask;
        });
    }

    private static bool TryParseVersionFromRelativePath(string? relativePath, out ApiVersion version)
    {
        version = default!;

        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        var routePath = relativePath.Split('?', 2)[0];
        var segments = routePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        foreach (var segment in segments)
            if (TryParseVersionSegment(segment, out version))
                return true;

        return false;
    }

    private static bool TryParseVersionSegment(string segment, out ApiVersion version)
    {
        version = default!;

        if (!segment.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            return false;

        var versionText = segment[1..];
        var versionParts = versionText.Split('.', StringSplitOptions.RemoveEmptyEntries);

        if (versionParts.Length == 1 &&
            int.TryParse(versionParts[0], out var majorVersion) &&
            majorVersion > 0)
        {
            version = new ApiVersion(majorVersion, 0);
            return true;
        }

        if (versionParts.Length == 2 &&
            int.TryParse(versionParts[0], out majorVersion) &&
            majorVersion > 0 &&
            int.TryParse(versionParts[1], out var minorVersion) &&
            minorVersion >= 0)
        {
            version = new ApiVersion(majorVersion, minorVersion);
            return true;
        }

        return false;
    }
}