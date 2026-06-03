using Asp.Versioning;
using Microsoft.AspNetCore.Builder;

namespace Matterway.ServiceDefaults.Bootstraps;

public readonly record struct ApiSpecVersionMetadata : IComparable<ApiSpecVersionMetadata>
{
    public int MajorVersion { get; }
    public int MinorVersion { get; }
    public int PatchVersion { get; }

    public ApiSpecVersionMetadata(int majorVersion, int minorVersion = 0, int patchVersion = 0)
    {
        if (majorVersion <= 0)
            throw new ArgumentOutOfRangeException(nameof(majorVersion), "API major version must be positive.");
        if (minorVersion < 0)
            throw new ArgumentOutOfRangeException(nameof(minorVersion), "API minor version cannot be negative.");
        if (patchVersion < 0)
            throw new ArgumentOutOfRangeException(nameof(patchVersion), "API patch version cannot be negative.");

        MajorVersion = majorVersion;
        MinorVersion = minorVersion;
        PatchVersion = patchVersion;
    }

    public int CompareTo(ApiSpecVersionMetadata other)
    {
        var majorComparison = MajorVersion.CompareTo(other.MajorVersion);
        if (majorComparison != 0)
            return majorComparison;

        var minorComparison = MinorVersion.CompareTo(other.MinorVersion);
        return minorComparison != 0
            ? minorComparison
            : PatchVersion.CompareTo(other.PatchVersion);
    }

    public override string ToString()
    {
        return $"{MajorVersion}.{MinorVersion}.{PatchVersion}";
    }
}

public static class EndpointVersionExtensions
{
    public static RouteHandlerBuilder MapToApiVersion(
        this RouteHandlerBuilder builder,
        int majorVersion,
        int minorVersion = 0,
        int patchVersion = 0)
    {
        var specVersion = new ApiSpecVersionMetadata(majorVersion, minorVersion, patchVersion);
        return builder
            .MapToApiVersion(new ApiVersion(majorVersion))
            .WithMetadata(specVersion);
    }
}