namespace Matterway.Catalog.Api.Domain;

public class ProductImage
{
    public required int Id { get; set; }
    public required Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public required string ImageRef { get; set; }
    public required string ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
}
