namespace Matterway.Catalog.Api.Domain.Entities;

public class ArticleImage
{
    public required Guid Id { get; init; } = Guid.CreateVersion7();
    public required int OrderIndex { get; set; }
    public required string ImageUrl { get; set; }
    public string? ImageAlt { get; set; }

    public required string ArticleCode { get; set; }
    public Article? Article { get; set; }
}