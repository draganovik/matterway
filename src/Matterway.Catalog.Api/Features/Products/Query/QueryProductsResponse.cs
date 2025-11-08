namespace Matterway.Catalog.Api.Features.Products.Query;

public record QueryProductResponse
{
    public Guid Id { get; set; }
    public string? ProductCode { get; set; }
    public string? Title { get; set; }
    public double? Price { get; set; }
    public string? Description { get; set; }
    public ProductImageProperty? ThumbnailImage { get; set; }
    public bool IsAvailable { get; set; }
}

public record ProductImageProperty
{
    public int Id { get; set; }
    public string? ImageRef { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
}
