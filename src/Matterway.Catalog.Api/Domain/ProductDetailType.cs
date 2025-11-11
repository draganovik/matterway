namespace Matterway.Catalog.Api.Domain;

public class ProductDetailType
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public string? Unit { get; init; }
}