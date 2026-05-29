namespace Matterway.Catalog.Api.Features.Public.Details.Contracts;

public sealed record PublicQueryDetailRequest : PaginationRequestParameters
{
    public string? TitleLike { get; init; }
}