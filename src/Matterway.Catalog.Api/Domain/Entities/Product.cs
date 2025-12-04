namespace Matterway.Catalog.Api.Domain.Entities;

public class Product
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string ProductCode { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<Price> Prices { get; } = [];
    public List<ProductDetail> ProductDetails { get; } = [];
    public List<ProductSpecification> ProductSpecifications { get; } = [];
    public List<ProductImage> ProductImages { get; } = [];

    public void UpdateDetails(string? productCode, string? title, string? description)
    {
        if (!string.IsNullOrWhiteSpace(productCode))
            ProductCode = productCode;

        if (!string.IsNullOrWhiteSpace(title))
            Title = title;

        if (!string.IsNullOrWhiteSpace(description))
            Description = description;

        Touch();
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        Touch();
    }

    public void SetPrice(ESupportedCurrency currency, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Price amount must be greater than zero.");

        var price = Prices.FirstOrDefault(p => p.Currency == currency);
        if (price is null)
        {
            price = new Price { ProductId = Id, Currency = currency, Amount = amount };
            Prices.Add(price);
        }
        else
        {
            price.Amount = amount;
        }

        Touch();
    }

    public decimal? GetBasePrice(ESupportedCurrency currency)
    {
        return Prices.FirstOrDefault(p => p.Currency == currency)?.Amount;
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

        return Prices
            .FirstOrDefault(p => p.Currency == currency)?
            .Discounts
            .Where(d => d.ValidFrom <= referenceTime && (d.ValidTo == null || d.ValidTo >= referenceTime))
            .OrderByDescending(d => d.Percentage)
            .ThenByDescending(d => d.ValidFrom)
            .FirstOrDefault();
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}