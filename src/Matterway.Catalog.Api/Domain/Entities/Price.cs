namespace Matterway.Catalog.Api.Domain.Entities;

public class Price
{
    public required ESupportedCurrency Currency { get; init; }
    public required double Amount { get; set; }

    public required Guid ProductId { get; init; }
    public Product? Product { get; init; }

    public readonly List<Discount> Discounts = [];
}