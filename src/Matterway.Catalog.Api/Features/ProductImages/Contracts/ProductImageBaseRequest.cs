using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.ProductImages.Contracts;

public class ProductImageBaseRequest
{
    [Required]
    public int Id { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Url]
    public string? ImageUrl { get; set; }

    [Required]
    public string? ImageAlt { get; set; }

    public bool IsMain { get; set; }
}