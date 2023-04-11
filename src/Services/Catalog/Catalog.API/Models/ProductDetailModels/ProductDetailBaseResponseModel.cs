using Shared.Enums;
using System.Text.Json.Serialization;

namespace Catalog.API.Models.ProductDetailModels;

public class ProductDetailBaseResponseModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductTitle { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DetailType Type { get; set; }
    public string? Title { get; set; }
    public string? Value { get; set; }
    public string? Unit { get; set; }
}
