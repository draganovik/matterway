namespace Matterway.Catalog.Api.Features.Admin.CatalogArchive.Contracts;

public record AdminImportCatalogArchiveResponse
{
    public int DetailCount { get; init; }
    public int ArticleCount { get; init; }
    public int DiscountCount { get; init; }
    public int ArticleDetailTextCount { get; init; }
    public int ArticleDetailNumericCount { get; init; }
    public int ArticleImageCount { get; init; }
}
