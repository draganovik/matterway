using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Common.Infrastructure.Pagination;

public class PaginationResponse<T>
{
    public PaginationResponse(int totalCount, int currentPage, int pageSize, List<T> data, Uri baseUrl)
    {
        Data = data;
        Meta = new PaginationMeta
        (
            totalCount,
            CurrentPage: currentPage,
            PageSize: pageSize,
            TotalPages: (int)Math.Ceiling((double)totalCount / pageSize)
        );
        Links = new PaginationLinks
        (
            GetPageUrl(1),
            GetPageUrl(Meta.TotalPages),
            currentPage > 1 ? GetPageUrl(currentPage - 1) : null,
            currentPage < Meta.TotalPages ? GetPageUrl(currentPage + 1) : null
        );

        string GetPageUrl(int page)
        {
            var uriBuilder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["page"] = page.ToString();
            query["pageSize"] = pageSize.ToString();
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.ToString();
        }
    }

    public PaginationMeta Meta { get; set; }
    public List<T> Data { get; set; }
    public PaginationLinks Links { get; set; }
}

public record PaginationLinks(
    [Url]
    string? First,
    [Url]
    string? Last,
    [Url]
    string? Prev,
    [Url]
    string? Next);

public record PaginationMeta(int TotalCount, int TotalPages, int CurrentPage, int PageSize);