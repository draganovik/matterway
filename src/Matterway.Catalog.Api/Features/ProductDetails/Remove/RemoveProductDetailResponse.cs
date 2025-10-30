namespace Matterway.Catalog.Api.Features.ProductDetails.Remove;

public record RemoveProductDetailResponse
{
    public required string DetailType { get; init; }
    public required Guid ProductId { get; init; }
    public string Message { get; init; } = "Product detail removed successfully.";
}