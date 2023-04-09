using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models.ProductImageModels;

public class ProductImageBaseRequestModel
{
    [Required]
    public int Id { get; set; }
    [Required]
    public Guid ProductId { get; set; }
    [Required]
    public string? ImageUrl { get; set; }
    [Required]
    public string? ImageAlt { get; set; }
    public bool IsMain { get; set; } = false;
}
