using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Entities;

public class Product
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string? ProductCode { get; set; }
    [Required]
    public string? Title { get; set; }
    [Required]
    public string? Description { get; set; }
    public ICollection<ProductDetail>? ProductDetails { get; set; }
    [Required]
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public bool IsAvailable { get; set; } = false;
}
