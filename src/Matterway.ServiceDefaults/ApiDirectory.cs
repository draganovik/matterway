using Asp.Versioning;

namespace Matterway.ServiceDefaults;

public sealed record ApiDefinition(string ServiceName, IReadOnlyCollection<ApiVersion> SupportedApiVersions)
{
    public string AspireServiceName => $"{ServiceName}-api";
    public string ServiceConfigurationPath => $"Services:{ToPascalCase(ServiceName)}";

    private static string ToPascalCase(string value)
    {
        var parts = value.Split(['-', '_'], StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Select(static part =>
        {
            return part.Length switch
            {
                0 => string.Empty,
                1 => part.ToUpperInvariant(),
                _ => char.ToUpperInvariant(part[0]) + part[1..].ToLowerInvariant()
            };
        }));
    }
}

public static class ApiDirectory
{
    public static readonly ApiDefinition Catalog = new(
        "catalog",
        [new ApiVersion(1)]);

    public static readonly ApiDefinition Customers = new(
        "customers",
        [new ApiVersion(1)]);

    public static readonly ApiDefinition Identity = new(
        "identity",
        [new ApiVersion(1)]);

    public static readonly ApiDefinition Sales = new(
        "sales",
        [new ApiVersion(1)]);

    public static IReadOnlyList<ApiDefinition> All { get; } = [Catalog, Customers, Identity, Sales];
}