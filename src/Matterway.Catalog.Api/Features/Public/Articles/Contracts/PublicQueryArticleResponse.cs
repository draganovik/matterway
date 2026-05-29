namespace Matterway.Catalog.Api.Features.Public.Articles.Contracts;

public record PublicQueryArticleResponse
{
    public required ArticleCode Code { get; set; }
    public string? Title { get; set; }
    public decimal? BasePrice { get; set; }
    public decimal? Price { get; set; }
    public DiscountResponse? Discount { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ThumbnailAlt { get; set; }
    public bool IsAvailable { get; set; }

    public record DiscountResponse
    {
        public decimal Percentage { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
    }
}