using System.ComponentModel.DataAnnotations;
using Catalog.API.Features.ProductDetails.Domain;
using Catalog.API.Features.ProductImages.Domain;

namespace Catalog.API.Features.Products.Domain;

public class Product
{
    [Key]
    public Guid Id { get; set; }

    [RegularExpression(@"^[A-Z0-9]{5,10}$",
        ErrorMessage = "Product code must be 5-10 characters and only contain uppercase letters and numbers.")]
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