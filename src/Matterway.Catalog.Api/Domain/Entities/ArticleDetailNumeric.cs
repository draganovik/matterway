namespace Matterway.Catalog.Api.Domain.Entities;

public class ArticleDetailNumeric
{
    public required decimal Value { get; set; }

    public required string ArticleCode { get; init; }
    public Article? Article { get; init; }
    public required string DetailSlug { get; init; }
    public Detail? Detail { get; init; }
}