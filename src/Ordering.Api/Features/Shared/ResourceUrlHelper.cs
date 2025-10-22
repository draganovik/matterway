namespace Ordering.Api.Features.Shared;

internal static class ResourceUrlHelper
{
    private const string DefaultVersionSegment = "v1.0";

    private static string GetVersionSegment(HttpContext httpContext)
    {
        var version = httpContext.GetRequestedApiVersion();
        return version is null ? DefaultVersionSegment : $"v{version}";
    }

    public static Uri CreateBaseUri(HttpContext httpContext, string resource)
    {
        var versionSegment = GetVersionSegment(httpContext);
        var resourcePath = resource.TrimStart('/');
        var baseUrl =
            $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/{versionSegment}/{resourcePath}";

        return new Uri(baseUrl);
    }

    public static string BuildResourceLocation(HttpContext httpContext, string resourcePath)
    {
        var versionSegment = GetVersionSegment(httpContext);
        return $"/api/{versionSegment}/{resourcePath.TrimStart('/')}";
    }
}