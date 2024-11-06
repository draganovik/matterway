using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Shared.Enums;

namespace Catalog.API.Entities;

public class ProductDetail
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product? Product { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DetailType Type { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Value { get; set; }

    public string? Unit { get; set; }
}