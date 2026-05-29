namespace Matterway.Catalog.Api.Features.Admin.ArticleImages.Contracts;

public record AdminUpdateArticleImageResponse
{
    public ICollection<AdminBaseArticleImageResponse>? Images { get; init; } = [];
    public required Guid UpdatedImageId { get; init; }
}