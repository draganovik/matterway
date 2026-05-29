using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Admin.Articles.Contracts;

public record AdminCreateArticleRequest
{
    [Required]
    public required ArticleCode Code { get; init; }

    [Required]
    public required string Title { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Base price must be greater than zero.")]
    public required decimal BasePrice { get; init; }

    [Required]
    public required string Description { get; init; }

    public bool IsAvailable { get; init; }
}