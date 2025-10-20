using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Features.ProductImages.Contracts;

public class ProductImageBaseResponse
{
    public int Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    public string? ImageAlt { get; set; }
    public bool IsMain { get; set; }
}