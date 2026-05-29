using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Features.Public.Articles.Contracts;

public record PublicGetArticleByCodeResponse
{
    public required ArticleCode Code { get; init; }
    public string? Title { get; init; }
    public decimal? BasePrice { get; init; }
    public decimal? Price { get; init; }
    public DiscountResponse? Discount { get; set; }
    public string? Description { get; init; }
    public ICollection<DetailResponse>? Details { get; init; } = [];
    public ICollection<ImageResponse>? Images { get; init; } = [];
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsAvailable { get; init; }

    public record DiscountResponse
    {
        public decimal Percentage { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
    }

    public record DetailResponse
    {
        public string? DetailSlug { get; init; }
        public string? Title { get; init; }
        public string? Unit { get; init; }
        public string? TextValue { get; init; }
        public decimal? NumericValue { get; init; }
    }

    public record ImageResponse
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
    }
}
