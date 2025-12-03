using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Domain.Entities;

public class Product
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string ProductCode { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required bool IsAvailable { get; set; } = false;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public readonly List<Price> Prices = [];
    public readonly List<ProductDetail> ProductDetails = [];
    public readonly List<ProductSpecification> ProductSpecifications = [];
    public readonly List<ProductImage> ProductImages = [];

    public decimal? GetBasePrice(ESupportedCurrency currency)
    {
        return Prices?.FirstOrDefault(p => p.Currency == currency)?.Amount;
    }

    public decimal? GetFinalPrice(ESupportedCurrency currency)
    {
        var basePrice = GetBasePrice(currency);
        if (basePrice is null) return null;

        var discount = GetLatestActiveDiscount(currency);

        if (discount is null) return basePrice;

        return basePrice.Value * (1 - discount.Percentage);
    }

    public Discount? GetLatestActiveDiscount(ESupportedCurrency currency, DateTime? now = null)
    {
        var referenceTime = now ?? DateTime.UtcNow;

        return Prices?
            .FirstOrDefault(p => p.Currency == currency)?
            .Discounts
            .Where(d => d.ValidFrom <= referenceTime && (d.ValidTo == null || d.ValidTo >= referenceTime))
            .OrderByDescending(d => d.Percentage)
            .ThenByDescending(d => d.ValidFrom)
            .FirstOrDefault();
    }
}