using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.ProductImages.Update;

public record UpdateProductImageRequest
{
    public string? ImageAlt { get; init; }

    [Range(0, int.MaxValue)]
    public int? OrderIndex { get; init; }
}
