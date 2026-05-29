using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Admin.ArticleImages.Contracts;

public record AdminAddArticleImageRequest
{
    [Required]
    [Range(0, int.MaxValue)]
    public int OrderIndex { get; init; }

    [Required]
    public IFormFile? File { get; init; }

    public string? ImageAlt { get; init; }
}
