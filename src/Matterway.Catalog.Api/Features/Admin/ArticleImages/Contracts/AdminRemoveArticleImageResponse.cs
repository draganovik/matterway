namespace Matterway.Catalog.Api.Features.Admin.ArticleImages.Contracts;

public record AdminRemoveArticleImageResponse
{
    public ICollection<AdminBaseArticleImageResponse>? Images { get; init; } = [];
    public required Guid DeletedImageId { get; init; }
}