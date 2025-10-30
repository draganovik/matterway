using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Matterway.Common.Enums;

namespace Matterway.Catalog.Api.Features.ProductDetails.Update;

public record UpdateProductDetailRequest
{
    [Required]
    public Guid ProductId { get; init; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DetailType Type { get; init; }

    [Required]
    public string? Title { get; init; }

    [Required]
    public string? Value { get; init; }

    public string? Unit { get; init; }
}