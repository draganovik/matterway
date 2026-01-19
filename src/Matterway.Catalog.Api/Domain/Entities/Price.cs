namespace Matterway.Catalog.Api.Domain.Entities;

public class Price
{
    public required ESupportedCurrency Currency { get; init; }
    public required decimal Amount { get; set; }

    public required Guid ArticleId { get; init; }
    public Article? Article { get; init; }

    public List<Discount> Discounts { get; } = [];
}