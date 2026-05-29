namespace Matterway.Catalog.Api.Features.Admin.ArticleDetails.Contracts;

public record AdminUpdateArticleDetailRequest
{
    public string? TextValue { get; init; }
    public decimal? NumericValue { get; init; }
}