using Matterway.Common.Enums;

namespace Matterway.Catalog.Api.Domain;

public class ProductDetail
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required Guid ProductId { get; init; }
    public Product? Product { get; init; }
    public required DetailType Type { get; set; }
    public required string Title { get; set; }
    public required string Value { get; set; }
    public string? Unit { get; set; }
}