using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Products.Update;

public class UpdateProductByIdRequest
{
    [StringLength(10, MinimumLength = 5, ErrorMessage = "Product code must be between 5 and 10 characters.")]
    [RegularExpression(@"^[A-Z0-9]{5,10}$",
        ErrorMessage = "Product code must contain only uppercase letters and numbers.")]
    public string? ProductCode { get; init; }

    [MinLength(1, ErrorMessage = "Title cannot be empty if provided.")]
    public string? Title { get; init; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public double? Price { get; init; }

    [MinLength(1, ErrorMessage = "Description cannot be empty if provided.")]
    public string? Description { get; init; }

    public bool IsAvailable { get; init; }
}