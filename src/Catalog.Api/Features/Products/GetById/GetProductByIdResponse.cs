using System.ComponentModel.DataAnnotations;
using Common.Infrastructure.Enums;

namespace Catalog.Api.Features.Products.GetById;

public record GetProductByIdResponse
{
    public Guid Id { get; set; }

    [RegularExpression(@"^[A-Z0-9]{5,10}$",
        ErrorMessage = "Product code must be 5-10 characters and only contain uppercase letters and numbers.")]
    public string? ProductCode { get; set; }

    public string? Title { get; set; }

    [Range(0.01, double.MaxValue)]
    public double? Price { get; set; }

    public string? Description { get; set; }
    public ICollection<ProductDetailProperty>? ProductDetails { get; set; } = [];
    public ICollection<ProductImageProperty>? ProductImages { get; set; } = [];
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsAvailable { get; set; }
}

public record ProductDetailProperty
{
    public Guid Id { get; set; }
    public DetailType Type { get; set; }
    public string? Title { get; set; }
    public string? Value { get; set; }
    public string? Unit { get; set; }
}

public record ProductImageProperty
{
    public int Id { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    public string? ImageAlt { get; set; }
    public bool IsMain { get; set; }
}