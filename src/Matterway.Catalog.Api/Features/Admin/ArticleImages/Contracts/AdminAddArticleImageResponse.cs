namespace Matterway.Catalog.Api.Features.Admin.ArticleImages.Contracts;

public record AdminAddArticleImageResponse
{
    public ICollection<AdminBaseArticleImageResponse>? Images { get; init; } = [];
    public required Guid CreatedImageId { get; init; }
}