using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Entities;

[PrimaryKey(nameof(Id), nameof(ProductId))]
public class ProductImage
{
    public int Id { get; set; }
    public Guid ProductId { get; set; }

    [Required]
    [Url]
    public string? ImageUrl { get; set; }

    [Required]
    public string? ImageAlt { get; set; } = "Product Image";

    public bool IsMain { get; set; } = false;

    [ForeignKey("ProductId")]
    public Product? Product { get; set; }
}