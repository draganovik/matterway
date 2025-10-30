namespace Matterway.Catalog.Api.Features.ProductImages.Remove;

public record RemoveProductImageResponse
{
    public required string ImageUrl { get; init; }
    public required Guid ProductId { get; init; }
    public string Message { get; init; } = "Product image removed successfully.";
}