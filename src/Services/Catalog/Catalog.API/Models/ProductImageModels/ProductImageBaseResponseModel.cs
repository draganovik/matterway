namespace Catalog.API.Models.ProductImageModels;

public class ProductImageBaseResponseModel
{
    public int Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
    public bool IsMain { get; set; }
}
