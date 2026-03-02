namespace Matterway.Catalog.Api.Domain.Entities;

public class Discount
{
    public required string Code { get; init; }
    public required decimal Percentage { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public required Guid ArticleId { get; init; }
    public Article? Article { get; init; }
}