namespace Shared.Models;

public class Product
{
    public Guid Id { get; set; }
    public string? ProductCode { get; set; }
    public string? Title { get; set; }
    public double? Price { get; set; }
    public string? Description { get; set; }
    public ICollection<ProductDetail>? ProductDetails { get; set; }
    public ICollection<ProductImage>? ProductImages { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsAvailable { get; set; }
}