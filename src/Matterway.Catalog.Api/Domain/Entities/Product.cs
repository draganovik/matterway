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
}