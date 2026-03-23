namespace Matterway.Catalog.Api.Infrastructure.Storage;

internal static class ImageStoragePaths
{
    public static string BuildObjectName(Guid imageId)
    {
        return $"images/{imageId:N}";
    }

    public static string BuildStorageUrl(string endpoint, string bucket, Guid imageId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(bucket);

        return $"{endpoint.TrimEnd('/')}/{bucket}/{BuildObjectName(imageId)}";
    }
}