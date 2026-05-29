using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Admin.ArticleDetails.Contracts;

public record AdminAddArticleDetailRequest
{
    [Required]
    public required string DetailSlug { get; init; }

    public string? TextValue { get; init; }
    public decimal? NumericValue { get; init; }
}
