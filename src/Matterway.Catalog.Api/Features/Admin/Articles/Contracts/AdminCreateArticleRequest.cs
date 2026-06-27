using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Admin.Articles.Contracts;

public record AdminCreateArticleRequest
{
    [Required]
    [ArticleCode]
    public string? Code { get; init; }

    [Required]
    public string? Title { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Base price must be greater than zero.")]
    public decimal? BasePrice { get; init; }

    [Required]
    public string? Description { get; init; }

    public bool IsAvailable { get; init; }
}