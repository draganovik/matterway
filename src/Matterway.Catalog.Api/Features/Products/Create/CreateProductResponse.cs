using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Products.Create;

public record CreateProductResponse
{
    public Guid Id { get; set; }

    [RegularExpression(@"^[A-Z0-9]{5,10}$",
        ErrorMessage = "Product code must be 5-10 characters and only contain uppercase letters and numbers.")]
    public string? ProductCode { get; set; }

    public string? Title { get; set; }

    [Range(0.01, double.MaxValue)]
    public double? Price { get; set; }

    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsAvailable { get; set; }
}