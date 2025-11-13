namespace Matterway.Common.Http;

public static class ApiResourceUriBuilder
{
    private const string ApiSegment = "/api";
    private const string DefaultApiVersion = "1.0";

    public static Uri BuildAbsoluteUri(HttpContext context, string resourcePath)
    {
        ArgumentNullException.ThrowIfNull(context);
        var relativePath = BuildRelativePath(context, resourcePath);

        var request = context.Request;
        var builder = new UriBuilder
        {
            Scheme = request.Scheme,
            Host = request.Host.Host
        };

        if (request.Host.Port is { } port)
        {
            builder.Port = port;
        }

        builder.Path = CombinePath(request.PathBase, relativePath);

        return builder.Uri;
    }

    public static string BuildRelativePath(HttpContext context, string resourcePath)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (string.IsNullOrWhiteSpace(resourcePath))
        {
            throw new ArgumentException("Resource path cannot be null or whitespace.", nameof(resourcePath));
        }

        var versionSegment = ResolveVersionSegment(context);
        var sanitizedResource = resourcePath.TrimStart('/');

        return $"{ApiSegment}/{versionSegment}/{sanitizedResource}";
    }

    private static string ResolveVersionSegment(HttpContext context)
    {
        var version = context.GetRequestedApiVersion();

        if (version is null)
        {
            return $"v{DefaultApiVersion}";
        }

        var formatted = version.ToString();

        return formatted.StartsWith("v", StringComparison.OrdinalIgnoreCase)
            ? formatted
            : $"v{formatted}";
    }

    private static string CombinePath(PathString pathBase, string relativePath)
    {
        var sanitizedRelative = relativePath.TrimStart('/');

        if (!pathBase.HasValue)
        {
            return $"/{sanitizedRelative}";
        }

        var sanitizedBase = pathBase.Value.TrimEnd('/');
        return $"{sanitizedBase}/{sanitizedRelative}";
    }
}