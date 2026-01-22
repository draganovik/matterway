namespace Matterway.Catalog.Api.Domain.Entities;

public class ArticleDetail
{
    public required string Value { get; set; }

    public required Guid ArticleId { get; init; }
    public Article? Article { get; init; }
    public required string DetailSlug { get; init; }
    public Detail? Detail { get; init; }
}