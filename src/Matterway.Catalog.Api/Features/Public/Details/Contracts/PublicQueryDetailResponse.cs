namespace Matterway.Catalog.Api.Features.Public.Details.Contracts;

public record PublicQueryDetailResponse
{
    public string? Slug { get; init; }
    public string? Title { get; init; }
    public string? Unit { get; init; }
}