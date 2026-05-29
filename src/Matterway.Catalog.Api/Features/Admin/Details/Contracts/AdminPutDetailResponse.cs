namespace Matterway.Catalog.Api.Features.Admin.Details.Contracts;

public record AdminPutDetailResponse
{
    public string? Slug { get; init; }
    public string? Title { get; init; }
    public string? Unit { get; init; }
}
