namespace Matterway.Catalog.Api.Domain.Entities;

public class Discount
{
    public required string Code { get; init; }
    public required decimal Percentage { get; init; }
    public string? Description { get; init; }
    public DateTime ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }

    public required Guid ProductId { get; init; }
    public ESupportedCurrency Currency { get; init; }
    public Price? Price { get; init; }
}