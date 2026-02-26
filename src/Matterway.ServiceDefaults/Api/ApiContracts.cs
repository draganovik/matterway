using Asp.Versioning;

namespace Matterway.ServiceDefaults.Api;

public sealed record ApiContract(string ServiceName, IReadOnlyCollection<ApiVersion> SupportedApiVersions);

public static class ApiContracts
{
    public static readonly ApiContract Catalog = new(
        "catalog",
        [new ApiVersion(1, 0), new ApiVersion(2, 0)]);

    public static readonly ApiContract Customers = new(
        "customers",
        [new ApiVersion(1, 0)]);

    public static readonly ApiContract Identity = new(
        "identity",
        [new ApiVersion(1, 0)]);

    public static readonly ApiContract Sales = new(
        "sales",
        [new ApiVersion(1, 0)]);

    public static IReadOnlyList<ApiContract> All { get; } = [Catalog, Customers, Identity, Sales];
}