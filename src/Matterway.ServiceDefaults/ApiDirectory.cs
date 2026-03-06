using Asp.Versioning;

namespace Matterway.ServiceDefaults;

public sealed record ApiDefinition(string ServiceName, IReadOnlyCollection<ApiVersion> SupportedApiVersions);

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