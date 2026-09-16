namespace Matterway.Catalog.Api.Features.Admin.ArticleDetails.Contracts;

public sealed record AdminPutArticleDetailRequest
{
    public string? TextValue { get; init; }
    public decimal? NumericValue { get; init; }
}