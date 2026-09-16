using Asp.Versioning;

namespace Matterway.ServiceDefaults;

public static class ApiVersioningConventions
{
    public static ApiVersion[] NormalizeSupportedVersions(IReadOnlyCollection<ApiVersion> supportedApiVersions)
    {
        if (supportedApiVersions.Count == 0)
            throw new ArgumentException("At least one supported API version is required.",
                nameof(supportedApiVersions));

        var routeVersions = supportedApiVersions
            .Select(ToRouteApiVersion)
            .ToArray();

        var duplicatedMajorVersion = routeVersions
            .GroupBy(GetMajorVersion)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicatedMajorVersion is not null)
            throw new ArgumentException(
                $"Only one API version per major version is supported. Major version {duplicatedMajorVersion.Key} is configured more than once.",
                nameof(supportedApiVersions));

        return routeVersions
            .OrderBy(GetMajorVersion)
            .ToArray();
    }

    public static string ToDocumentName(ApiVersion version)
    {
        return $"v{GetMajorVersion(version)}";
    }

    private static int GetMajorVersion(ApiVersion version)
    {
        return version.MajorVersion.GetValueOrDefault();
    }

    private static ApiVersion ToRouteApiVersion(ApiVersion version)
    {
        var majorVersion = GetMajorVersion(version);
        if (majorVersion <= 0)
            throw new ArgumentException("API major versions must be positive.");

        return new ApiVersion(majorVersion);
    }
}