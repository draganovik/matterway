namespace Matterway.Catalog.Api.Features.Admin.ArticleDetails.Contracts;

public record AdminUpdateArticleDetailResponse
{
    public required ArticleCode ArticleCode { get; init; }
    public string? ArticleTitle { get; init; }
    public string? DetailSlug { get; init; }
    public string? Title { get; init; }
    public string? Unit { get; init; }
    public string? TextValue { get; init; }
    public decimal? NumericValue { get; init; }
}