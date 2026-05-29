using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Admin.ArticleImages.Contracts;

public record AdminUpdateArticleImageRequest
{
    public string? ImageAlt { get; init; }

    [Range(0, int.MaxValue)]
    public int? OrderIndex { get; init; }
}