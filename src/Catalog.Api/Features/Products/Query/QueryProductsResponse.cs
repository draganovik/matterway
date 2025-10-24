using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.Features.Products.Query;

public record QueryProductResponse
{
    public Guid Id { get; set; }

    [RegularExpression(@"^[A-Z0-9]{5,10}$",
        ErrorMessage = "Product code must be 5-10 characters and only contain uppercase letters and numbers.")]
    public string? ProductCode { get; set; }

    public string? Title { get; set; }

    [Range(0.01, double.MaxValue)]
    public double? Price { get; set; }

    public string? Description { get; set; }

    [Required]
    public ProductImageProperty? ThumbnailImage { get; set; }

    public bool IsAvailable { get; set; }
}

public record ProductImageProperty
{
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
}