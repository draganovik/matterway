using Catalog.API.Models.ProductDetailModels;
using Catalog.API.Models.ProductImageModels;

namespace Catalog.API.Models.ProductModels;

public class ProductBaseResponseModel
{
    public Guid Id { get; set; }
    public string? ProductCode { get; set; }
    public string? Title { get; set; }
    public double? Price { get; set; }
    public string? Description { get; set; }
    public ICollection<ProductDetailProductResponseModel>? ProductDetails { get; set; }
    public ICollection<ProductImageProductResponseModel>? ProductImages { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsAvailable { get; set; }
}
