using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.WebUtilities;

namespace Matterway.ServiceDefaults.Api;

public record PaginationRequestParameters
{
    [Required(ErrorMessage = "The field Page is required and must be a valid number.")]
    [Range(1, int.MaxValue)]
    public int Page { get; init; }

    [Required(ErrorMessage = "The field PageSize is required and must be a valid number.")]
    [Range(1, int.MaxValue)]
    public int PageSize { get; init; }
}

public sealed record PaginationResponse<T>(
    PaginationResponseMeta Meta,
    IReadOnlyList<T> Data,
    PaginationResponseLinks? Links
)
{
    public static PaginationResponse<T> Create(
        IReadOnlyList<T> data,
        int totalCount,
        int currentPage,
        int pageSize,
        string? endpointUrl)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(currentPage);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var meta = new PaginationResponseMeta(
            totalCount,
            totalPages,
            currentPage,
            pageSize
        );

        var links = endpointUrl is null
            ? null
            : new PaginationResponseLinks(
                totalPages > 0 ? BuildUrl(1) : null,
                totalPages > 0 ? BuildUrl(totalPages) : null,
                currentPage > 1 ? BuildUrl(currentPage - 1) : null,
                currentPage < totalPages ? BuildUrl(currentPage + 1) : null
            );

        return new PaginationResponse<T>(meta, data, links);

        string BuildUrl(int page)
        {
            var pageQuery = QueryHelpers.AddQueryString(endpointUrl, "page", page.ToString());
            return QueryHelpers.AddQueryString(pageQuery, "pageSize", pageSize.ToString());
        }
    }
}

public sealed record PaginationResponseMeta(
    int TotalCount,
    int TotalPages,
    int CurrentPage,
    int PageSize
);

public sealed record PaginationResponseLinks(
    string? First,
    string? Last,
    string? Prev,
    string? Next
);