namespace Matterway.Catalog.Api.Domain.Entities;

public class ProductSpecification
{
    public required Guid ProductId { get; init; }
    public Product? Product { get; init; }
    public required string SpecificationSlug { get; init; }
    public Specification? Specification { get; init; }
    public required decimal Value { get; set; }
}