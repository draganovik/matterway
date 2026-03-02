using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.ImportExport.Rows;

public sealed record ArticleDetailTextRow(
    Guid ArticleId,
    string DetailSlug,
    string Value)
{
    public static ArticleDetailTextRow FromEntity(ArticleDetailText entity)
    {
        return new ArticleDetailTextRow(entity.ArticleId, entity.DetailSlug, entity.Value);
    }

    public ArticleDetailText ToEntity()
    {
        return new ArticleDetailText
        {
            ArticleId = ArticleId,
            DetailSlug = DetailSlug,
            Value = Value
        };
    }
}