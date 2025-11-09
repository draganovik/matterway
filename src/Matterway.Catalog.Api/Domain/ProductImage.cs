namespace Matterway.Catalog.Api.Domain;

public class ProductImage
{
    public required Guid Id { get; set; }
    public required Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public required int OrderIndex { get; set; }
    public required string ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
}