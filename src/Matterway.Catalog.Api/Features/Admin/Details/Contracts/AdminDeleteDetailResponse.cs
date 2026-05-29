namespace Matterway.Catalog.Api.Features.Admin.Details.Contracts;

public record AdminDeleteDetailResponse
{
    public string Slug { get; init; } = default!;
    public string Message { get; init; } = "Detail removed successfully.";
}
