namespace Matterway.Catalog.Api.Features.ProductImages.Update;

public record UpdateProductImageResponse
{
    public int Id { get; init; }
    public Guid ProductId { get; init; }
    public string? ProductName { get; init; }

    public string? ImageRef { get; init; }
    public string? ImageUrl { get; init; }

    public string? ImageAlt { get; init; }
}
