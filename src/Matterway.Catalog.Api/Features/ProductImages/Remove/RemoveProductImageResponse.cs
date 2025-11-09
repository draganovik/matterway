namespace Matterway.Catalog.Api.Features.ProductImages.Remove;

public record RemoveProductImageResponse
{
    public Guid Id { get; init; }
    public int OrderIndex { get; init; }
    public required string ImageUrl { get; init; }
    public string? ImageAlt { get; init; }
    public required Guid ProductId { get; init; }
    public string Message { get; init; } = "Product image removed successfully.";
}