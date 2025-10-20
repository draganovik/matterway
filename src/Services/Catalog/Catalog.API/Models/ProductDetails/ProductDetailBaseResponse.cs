using System.Text.Json.Serialization;
using Shared.Enums;

namespace Catalog.API.Models.ProductDetailModels;

public class ProductDetailBaseResponse
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