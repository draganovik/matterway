namespace Matterway.Catalog.Api.Application;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.WebUtilities;

public sealed record PaginationResponse<T>(
    PaginationMeta Meta,
    IReadOnlyList<T> Data,
    PaginationLinks Links
)
{
    public static PaginationResponse<T> Create(
        IReadOnlyList<T> data,
        int totalCount,
        int currentPage,
        int pageSize,
        string endpointUrl)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(currentPage);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        // Build metadata
        var meta = new PaginationMeta(
            totalCount,
            totalPages,
            currentPage,
            pageSize
        );

        // Build links
        var links = new PaginationLinks(
            totalPages > 0 ? BuildUrl(1) : null,
            totalPages > 0 ? BuildUrl(totalPages) : null,
            currentPage > 1 ? BuildUrl(currentPage - 1) : null,
            currentPage < totalPages ? BuildUrl(currentPage + 1) : null
        );

        return new PaginationResponse<T>(meta, data, links);

        // Local helper
        string BuildUrl(int page)
        {
            var s1 = QueryHelpers.AddQueryString(endpointUrl, "page", page.ToString());
            var s2 = QueryHelpers.AddQueryString(s1, "pageSize", pageSize.ToString());
            return s2;
        }
    }
}

public sealed record PaginationMeta(
    int TotalCount,
    int TotalPages,
    int CurrentPage,
    int PageSize
);

public sealed record PaginationLinks(
    [Url]
    string? First,
    [Url]
    string? Last,
    [Url]
    string? Prev,
    [Url]
    string? Next
);

public sealed record PaginationQuery
{
    [Required(ErrorMessage = "The field Page is required and must be a valid number.")]
    [Range(1, int.MaxValue)]
    public int Page { get; init; }

    [Required(ErrorMessage = "The field PageSize is required and must be a valid number.")]
    [Range(1, int.MaxValue)]
    public int PageSize { get; init; }
}