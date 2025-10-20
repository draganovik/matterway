using System.ComponentModel.DataAnnotations;
using Catalog.API.Features.ProductDetails.Contracts;
using Catalog.API.Features.ProductImages.Contracts;

namespace Catalog.API.Features.Products.Contracts;

public class ProductBaseResponse
{
    public Guid Id { get; set; }

    [RegularExpression(@"^[A-Z0-9]{5,10}$",
        ErrorMessage = "Product code must be 5-10 characters and only contain uppercase letters and numbers.")]
    public string? ProductCode { get; set; }

    public string? Title { get; set; }

    [Range(0.01, double.MaxValue)]
    public double? Price { get; set; }

    public string? Description { get; set; }
    public ICollection<ProductDetailProductResponse>? ProductDetails { get; set; }
    public ICollection<ProductImageProductResponse>? ProductImages { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsAvailable { get; set; }
}