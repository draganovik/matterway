namespace Matterway.Catalog.Api.Features.Admin.ArticleDetails.Contracts;

public record AdminRemoveArticleDetailResponse
{
    public required ArticleCode ArticleCode { get; init; }
    public required string DetailSlug { get; init; }
    public string? Title { get; init; }
    public string? Unit { get; init; }
    public string Message { get; init; } = "Article detail removed successfully.";
}