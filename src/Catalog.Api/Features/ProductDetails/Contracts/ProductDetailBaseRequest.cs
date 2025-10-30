using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Matterway.Common.Enums;

namespace Catalog.Api.Features.ProductDetails.Contracts;

public class ProductDetailBaseRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DetailType Type { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Value { get; set; }

    public string? Unit { get; set; }
}