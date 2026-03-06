using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.ImportExport.Rows;

public sealed record ArticleDetailNumericRow(
    string ArticleCode,
    string DetailSlug,
    decimal Value)
{
    public static ArticleDetailNumericRow FromEntity(ArticleDetailNumeric entity)
    {
        return new ArticleDetailNumericRow(entity.ArticleCode, entity.DetailSlug, entity.Value);
    }

    public ArticleDetailNumeric ToEntity()
    {
        return new ArticleDetailNumeric
        {
            ArticleCode = ArticleCode,
            DetailSlug = DetailSlug,
            Value = Value
        };
    }
}