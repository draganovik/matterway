namespace Matterway.Catalog.Api.Domain;

public class ProductDetailType
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public string? Unit { get; init; }
}