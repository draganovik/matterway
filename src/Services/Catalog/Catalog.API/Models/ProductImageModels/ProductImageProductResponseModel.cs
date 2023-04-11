using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models.ProductImageModels;

public class ProductImageProductResponseModel
{
    [Url]
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
    public bool IsMain { get; set; }
}
