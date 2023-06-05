using Shared.Enums;
using System.Text.Json.Serialization;

namespace Catalog.API.Models.ProductDetailModels;

public class ProductDetailProductResponseModel
{
    public Guid Id { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DetailType Type { get; set; }
    public string? Title { get; set; }
    public string? Value { get; set; }
    public string? Unit { get; set; }
}
