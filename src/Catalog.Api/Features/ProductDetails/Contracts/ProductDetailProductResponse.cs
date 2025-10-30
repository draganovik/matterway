using System.Text.Json.Serialization;
using Matterway.Common.Enums;

namespace Catalog.Api.Features.ProductDetails.Contracts;

public class ProductDetailProductResponse
{
    public Guid Id { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DetailType Type { get; set; }

    public string? Title { get; set; }
    public string? Value { get; set; }
    public string? Unit { get; set; }
}