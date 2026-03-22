namespace Matterway.Catalog.Api.Infrastructure.Storage;

internal static class CatalogImagePaths
{
    private const string PublicImagePathPrefix = "/api/catalog/public/v1/images";

    public static string BuildObjectName(Guid imageId)
    {
        return $"images/{imageId:N}";
    }

    public static string BuildPublicUrl(Guid imageId)
    {
        return $"{PublicImagePathPrefix}/{imageId:N}";
    }
}