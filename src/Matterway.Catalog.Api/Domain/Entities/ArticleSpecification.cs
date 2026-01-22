namespace Matterway.Catalog.Api.Domain.Entities;

public class ArticleSpecification
{
    public required decimal Value { get; set; }

    public required Guid ArticleId { get; init; }
    public Article? Article { get; init; }
    public required string SpecificationSlug { get; init; }
    public Specification? Specification { get; init; }
}