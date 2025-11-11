using Matterway.Common.Enums;

namespace Matterway.Catalog.Api.Domain;

public class ProductDetail
{
    public required Guid ProductId { get; init; }
    public Product? Product { get; init; }
    public required int TypeId { get; init; }
    public ProductDetailType? Type { get; init; }
    public required string Value { get; set; }
}