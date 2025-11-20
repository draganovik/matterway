namespace Matterway.Catalog.Api.Domain.Entities;

public class Product
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string ProductCode { get; set; }
    public required string Title { get; set; }
    public required double Price { get; set; }
    public required string Description { get; set; }
    public ICollection<ProductDetail>? ProductDetails { get; init; }
    public ICollection<ProductImage>? ProductImages { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public required bool IsAvailable { get; set; } = false;
}