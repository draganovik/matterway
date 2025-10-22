using System.Text.Json.Serialization;
using Common.Infrastructure.Enums;

namespace Catalog.API.Features.ProductDetails.Contracts;

public class ProductDetailProductResponse
{
    public Guid Id { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DetailType Type { get; set; }

    public string? Title { get; set; }
    public string? Value { get; set; }
    public string? Unit { get; set; }
}