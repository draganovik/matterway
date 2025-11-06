using Matterway.Common.Enums;

namespace Matterway.Catalog.Api.Features.Products.GetById;

public record GetProductByIdResponse
{
    public Guid Id { get; init; }
    public string? ProductCode { get; init; }
    public string? Title { get; init; }
    public double? Price { get; init; }
    public string? Description { get; init; }
    public ICollection<ProductDetailProperty>? ProductDetails { get; init; } = [];
    public ICollection<ProductImageProperty>? ProductImages { get; init; } = [];
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsAvailable { get; init; }
}

public record ProductDetailProperty
{
    public Guid Id { get; init; }
    public DetailType Type { get; init; }
    public string? Title { get; init; }
    public string? Value { get; init; }
    public string? Unit { get; init; }
}

public record ProductImageProperty
{
    public int Id { get; init; }
    public string? ImageUrl { get; init; }
    public string? ImageAlt { get; init; }
    public bool IsMain { get; init; }
}