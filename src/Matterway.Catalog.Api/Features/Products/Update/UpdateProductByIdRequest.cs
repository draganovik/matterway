using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Products.Update;

public class UpdateProductByIdRequest
{
    [RegularExpression(@"^[A-Z0-9]{5,10}$",
        ErrorMessage = "Product code must be 5-10 characters and only contain uppercase letters and numbers.")]
    public string? ProductCode { get; init; }

    public string? Title { get; init; }

    [Range(0.01, double.MaxValue)]
    public double? Price { get; init; }

    public string? Description { get; init; }
    public bool IsAvailable { get; init; }
}