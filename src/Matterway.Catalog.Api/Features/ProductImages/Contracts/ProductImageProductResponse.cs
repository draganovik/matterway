using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.ProductImages.Contracts;

public class ProductImageProductResponse
{
    public int Id { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    public string? ImageAlt { get; set; }
    public bool IsMain { get; set; }
}