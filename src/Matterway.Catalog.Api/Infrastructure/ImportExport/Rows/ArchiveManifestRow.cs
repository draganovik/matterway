namespace Matterway.Catalog.Api.Infrastructure.ImportExport.Rows;

public sealed record ArchiveManifestRow(
    int Version,
    DateTime ExportedAtUtc,
    int DetailCount,
    int ArticleCount,
    int DiscountCount,
    int ArticleDetailTextCount,
    int ArticleDetailNumericCount,
    int ArticleImageCount)
{
    public static ArchiveManifestRow Create(
        int detailCount,
        int articleCount,
        int discountCount,
        int articleDetailTextCount,
        int articleDetailNumericCount,
        int articleImageCount)
    {
        return new ArchiveManifestRow(
            1,
            DateTime.UtcNow,
            detailCount,
            articleCount,
            discountCount,
            articleDetailTextCount,
            articleDetailNumericCount,
            articleImageCount);
    }
}