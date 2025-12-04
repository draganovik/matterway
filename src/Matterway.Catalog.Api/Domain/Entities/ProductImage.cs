namespace Matterway.Catalog.Api.Domain.Entities;

public class ProductImage
{
    public required Guid Id { get; init; } = Guid.CreateVersion7();
    public required int OrderIndex { get; set; }
    public required string ImageUrl { get; set; }
    public string? ImageAlt { get; set; }

    public required Guid ProductId { get; set; }
    public Product? Product { get; set; }
}