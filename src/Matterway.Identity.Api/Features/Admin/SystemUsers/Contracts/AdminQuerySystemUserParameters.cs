namespace Matterway.Identity.Api.Features.Admin.SystemUsers.Contracts;

public sealed record AdminQuerySystemUserParameters : PaginationRequestParameters
{
    public string? Role { get; init; }
}