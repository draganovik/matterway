namespace Matterway.Catalog.Api.Features.Products.Update;

public record UpdateProductByIdResponse
{
    public Guid Id { get; init; }
    public string? ProductCode { get; init; }
    public string? Title { get; init; }
    public double? Price { get; init; }
    public string? Description { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsAvailable { get; init; }
}