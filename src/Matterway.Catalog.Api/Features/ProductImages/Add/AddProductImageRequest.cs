using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.ProductImages.Add;

public record AddProductImageRequest
{
    [Required]
    [Range(0, int.MaxValue)]
    public int Id { get; init; }

    [Required]
    public Guid ProductId { get; init; }

    [Required]
    public IFormFile? File { get; init; }

    public string? ImageAlt { get; init; }
}