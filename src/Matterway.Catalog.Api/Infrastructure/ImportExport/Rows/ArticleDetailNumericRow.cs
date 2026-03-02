using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.ImportExport.Rows;

public sealed record ArticleDetailNumericRow(
    Guid ArticleId,
    string DetailSlug,
    decimal Value)
{
    public static ArticleDetailNumericRow FromEntity(ArticleDetailNumeric entity)
    {
        return new ArticleDetailNumericRow(entity.ArticleId, entity.DetailSlug, entity.Value);
    }

    public ArticleDetailNumeric ToEntity()
    {
        return new ArticleDetailNumeric
        {
            ArticleId = ArticleId,
            DetailSlug = DetailSlug,
            Value = Value
        };
    }
}