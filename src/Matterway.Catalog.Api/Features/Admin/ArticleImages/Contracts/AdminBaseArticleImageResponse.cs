namespace Matterway.Catalog.Api.Features.Admin.ArticleImages.Contracts;

public record AdminBaseArticleImageResponse
{
    public Guid Id { get; init; }
    public int OrderIndex { get; init; }
    public string? ImageUrl { get; init; }
    public string? ImageAlt { get; init; }
}
