using System.Text.Json.Serialization;
using Matterway.Common.Enums;

namespace Matterway.Common.Models;

public class Product
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Title { get; set; }
    public double? BasePrice { get; set; }
    public double? Price { get; set; }
    public ProductDiscount? Discount { get; set; }
    public string? Description { get; set; }
    public ICollection<ProductDetail>? Details { get; set; }
    public ICollection<ProductSpecification>? Specifications { get; init; } = [];
    public ICollection<ProductImage>? Images { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ThumbnailAlt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsAvailable { get; set; }
}

public class ProductDiscount
{
    public double Percentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public record ProductDetail
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DetailType Type { get; set; }

    public string? Title { get; set; }
    public string? Value { get; set; }
    public string? Unit { get; set; }
}

public record ProductSpecification
{
    public string? SpecificationSlug { get; init; }
    public string? Title { get; init; }
    public decimal Value { get; init; }
    public string? Unit { get; init; }
}

public record ProductImage
{
    public Guid Id { get; set; }
    public int OrderIndex { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
}