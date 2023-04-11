using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Entities;

public class Product
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string? ProductCode { get; set; }
    [Required]
    public string? Title { get; set; }
    [Required]
    [Range(0.01, double.MaxValue)]
    public double? Price { get; set; }
    [Required]
    public string? Description { get; set; }
    public ICollection<ProductDetail>? ProductDetails { get; set; }
    public ICollection<ProductImage>? ProductImages { get; set; }
    [Required]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    [Required]
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    [Required]
    public bool IsAvailable { get; set; } = false;
}
