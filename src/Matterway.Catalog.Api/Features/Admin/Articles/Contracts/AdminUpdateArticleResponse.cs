using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Features.Admin.Articles.Contracts;

public record AdminUpdateArticleResponse
{
    public required ArticleCode Code { get; init; }
    public string? Title { get; init; }
    public decimal? BasePrice { get; init; }
    public decimal? Price { get; init; }
    public string? Description { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsAvailable { get; init; }
}
