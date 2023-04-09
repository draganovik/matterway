using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models.ProductModels;

public class ProductBaseRequestModel
{
    [Required]
    public string? ProductCode { get; set; }
    [Required]
    public string? Title { get; set; }
    [Required]
    [Range(0.01, double.MaxValue)]
    public double? Price { get; set; }
    [Required]
    public string? Description { get; set; }
    [Required]
    public bool IsAvailable { get; set; } = false;
}
