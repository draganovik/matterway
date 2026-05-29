namespace Matterway.Catalog.Api.Features.Public.Articles.Contracts;

public sealed record PublicQueryArticleParameters : PaginationRequestParameters
{
    /// <summary>
    /// RSQL filter string. Use ';' for AND, and ',' for OR. Text fields (details) support ==/!=/in/out;
    /// numeric fields (price, numeric details) support eq/!=/ge/le/in/out. Fields:
    /// title, code, description, price, available, and detail slugs.
    /// NOTE: eq and == are equivalent and validate if a field contains the given value for strings.
    /// </summary>
    public string? Filter { get; init; }
}
