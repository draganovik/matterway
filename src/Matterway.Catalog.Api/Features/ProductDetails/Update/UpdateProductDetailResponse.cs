using System.Text.Json.Serialization;
using Matterway.Common.Enums;

namespace Matterway.Catalog.Api.Features.ProductDetails.Update;

public record UpdateProductDetailResponse
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string? ProductTitle { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DetailType Type { get; init; }

    public string? Title { get; init; }
    public string? Value { get; init; }
    public string? Unit { get; init; }
}