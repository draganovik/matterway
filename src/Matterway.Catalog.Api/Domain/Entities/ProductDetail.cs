namespace Matterway.Catalog.Api.Domain.Entities;

public class ProductDetail
{
    public required Guid ProductId { get; init; }
    public Product? Product { get; init; }
    public required string DetailSlug { get; init; }
    public Detail? Detail { get; init; }
    public required string Value { get; set; }
}