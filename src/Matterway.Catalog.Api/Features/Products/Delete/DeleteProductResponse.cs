namespace Matterway.Catalog.Api.Features.Products.Delete;

public record DeleteProductResponse
{
    public Guid Id { get; init; }
    public string Message { get; init; } = "Product deleted successfully.";
}