namespace Matterway.Catalog.Api.Domain.Entities;

public class Article
{
    public required string ArticleCode { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<ArticleDetailText> ArticleDetailTexts { get; } = [];
    public List<ArticleDetailNumeric> ArticleDetailNumerics { get; } = [];
    public List<ArticleImage> ArticleImages { get; } = [];
    public List<Discount> Discounts { get; } = [];

    public void UpdateDetails(ArticleCode? code, string? title, string? description)
    {
        if (code is { } nextCode)
            ArticleCode = nextCode.Value;

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

    public void SetBasePrice(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Price amount must be greater than zero.");

        BasePrice = amount;
        Touch();
    }

    public decimal GetFinalPrice(DateTime? now = null)
    {
        var discount = GetLatestActiveDiscount(now);
        if (discount is null) return BasePrice;

        return BasePrice * (1 - discount.Percentage);
    }

    public Discount? GetLatestActiveDiscount(DateTime? now = null)
    {
        var referenceTime = now ?? DateTime.UtcNow;

        return Discounts
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