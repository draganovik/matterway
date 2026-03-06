using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.ImportExport.Rows;

public sealed record ArticleDetailTextRow(
    string ArticleCode,
    string DetailSlug,
    string Value)
{
    public static ArticleDetailTextRow FromEntity(ArticleDetailText entity)
    {
        return new ArticleDetailTextRow(entity.ArticleCode, entity.DetailSlug, entity.Value);
    }

    public ArticleDetailText ToEntity()
    {
        return new ArticleDetailText
        {
            ArticleCode = ArticleCode,
            DetailSlug = DetailSlug,
            Value = Value
        };
    }
}