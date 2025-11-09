namespace Matterway.Catalog.Api.Features.ProductImages.Add;

public record AddProductImageResponse
{
    public Guid Id { get; init; }
    public int OrderIndex { get; init; }
    public Guid ProductId { get; init; }
    public string? ProductName { get; init; }
    public string? ImageUrl { get; init; }
    public string? ImageAlt { get; init; }
}
