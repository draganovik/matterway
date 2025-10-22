using System.Text.Json.Serialization;
using Common.Infrastructure.Enums;

namespace Catalog.Api.Features.ProductDetails.Contracts;

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