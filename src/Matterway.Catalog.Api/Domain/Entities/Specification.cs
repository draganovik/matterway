namespace Matterway.Catalog.Api.Domain.Entities;

public class Specification
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public string? Unit { get; init; }
}