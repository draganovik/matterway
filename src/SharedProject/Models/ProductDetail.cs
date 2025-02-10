using System.Text.Json.Serialization;
using Shared.Enums;

namespace Shared.Models;

public class ProductDetail
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DetailType Type { get; set; }

    public string? Title { get; set; }
    public string? Value { get; set; }
    public string? Unit { get; set; }
}