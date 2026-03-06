using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.ImportExport.Rows;

public sealed record ArticleRow(
    string ArticleCode,
    string Title,
    string Description,
    decimal BasePrice,
    bool IsAvailable,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static ArticleRow FromEntity(Article entity)
    {
        return new ArticleRow(
            entity.ArticleCode,
            entity.Title,
            entity.Description,
            entity.BasePrice,
            entity.IsAvailable,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public Article ToEntity()
    {
        return new Article
        {
            ArticleCode = ArticleCode,
            Title = Title,
            Description = Description,
            BasePrice = BasePrice,
            IsAvailable = IsAvailable,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt
        };
    }
}