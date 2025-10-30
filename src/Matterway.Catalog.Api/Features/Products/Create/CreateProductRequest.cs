using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Products.Create;

public record CreateProductRequest
{
    [Required]
    [RegularExpression(@"^[A-Z0-9]{5,10}$",
        ErrorMessage = "Product code must be 5-10 characters and only contain uppercase letters and numbers.")]
    public string? ProductCode { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public double? Price { get; set; }

    [Required]
    public string? Description { get; set; }

    public bool IsAvailable { get; set; }
}