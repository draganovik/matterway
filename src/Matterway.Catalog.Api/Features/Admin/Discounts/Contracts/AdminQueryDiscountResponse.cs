namespace Matterway.Catalog.Api.Features.Admin.Discounts.Contracts;

public record AdminQueryDiscountResponse
{
    public string? Code { get; init; }
    public decimal Percentage { get; init; }
    public DateTime ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public required ArticleCode ArticleCode { get; init; }
}