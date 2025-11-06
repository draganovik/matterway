using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Products.Create;

public record CreateProductRequest
{
    [Required]
    [RegularExpression(@"^[A-Z0-9]{5,10}$",
        ErrorMessage = "Product code must be 5-10 characters and only contain uppercase letters and numbers.")]
    public required string ProductCode { get; init; }

    [Required]
    public required string Title { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public required double Price { get; init; }

    [Required]
    public required string Description { get; init; }

    public bool IsAvailable { get; init; }
}