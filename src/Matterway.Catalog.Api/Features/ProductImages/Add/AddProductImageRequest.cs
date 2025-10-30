using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.ProductImages.Add;

public record AddProductImageRequest
{
    [Required]
    public int Id { get; init; }

    [Required]
    public Guid ProductId { get; init; }

    [Required]
    [Url]
    public string? ImageUrl { get; init; }

    [Required]
    public string? ImageAlt { get; init; }

    public bool IsMain { get; init; }
}