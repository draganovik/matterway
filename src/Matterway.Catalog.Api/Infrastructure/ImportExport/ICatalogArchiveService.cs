namespace Matterway.Catalog.Api.Infrastructure.ImportExport;

public interface ICatalogArchiveService
{
    Task<ExportCatalogArchiveResult> ExportAsync(CancellationToken cancellationToken = default);
    Task<ImportCatalogArchiveResult> ImportAsync(Stream archiveStream, CancellationToken cancellationToken = default);
}

public sealed record ExportCatalogArchiveResult(
    byte[] Content,
    string FileName,
    int ArticleCount,
    int ImageCount);

public sealed record ImportCatalogArchiveResult(
    int DetailCount,
    int ArticleCount,
    int DiscountCount,
    int ArticleDetailTextCount,
    int ArticleDetailNumericCount,
    int ArticleImageCount);