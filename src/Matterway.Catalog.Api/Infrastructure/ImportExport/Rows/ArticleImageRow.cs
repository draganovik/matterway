using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.ImportExport.Rows;

public sealed record ArticleImageRow(
    Guid Id,
    string ArticleCode,
    int OrderIndex,
    string ImageAlt,
    string ImageFile,
    string ContentType)
{
    public static ArticleImageRow FromEntity(ArticleImage entity, string imageFile, string contentType)
    {
        return new ArticleImageRow(
            entity.Id,
            entity.ArticleCode,
            entity.OrderIndex,
            entity.ImageAlt ?? string.Empty,
            imageFile,
            contentType);
    }

    public ArticleImage ToEntity(string imageUrl)
    {
        return new ArticleImage
        {
            Id = Id,
            ArticleCode = ArticleCode,
            OrderIndex = OrderIndex,
            ImageAlt = ImageAlt ?? string.Empty,
            ImageUrl = imageUrl
        };
    }
}