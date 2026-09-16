using System.Net;
using System.Text.Json;

namespace Matterway.Sales.Api.Infrastructure.Brokers.Customers;

public class HttpCustomersClient(HttpClient httpClient) : ICustomersClient
{
    public async Task<BrokerResponse<CustomersOrderResponse>> PutOrderAsync(
        OrderId orderId,
        CustomersCreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var httpRequest =
            new HttpRequestMessage(HttpMethod.Put, $"/api/system/v1/orders/{orderId}")
            {
                Content = JsonContent.Create(request)
            };

        var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var rawError = await response.Content.ReadAsStringAsync(cancellationToken);
            var error = ExtractErrorMessage(rawError) ?? response.ReasonPhrase;
            return BrokerResponse<CustomersOrderResponse>.Failure(
                error,
                response.StatusCode);
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var order = await JsonSerializer.DeserializeAsync<CustomersOrderResponse>(contentStream, options,
            cancellationToken);

        return order is null
            ? BrokerResponse<CustomersOrderResponse>.Failure("Empty response from customers service.",
                HttpStatusCode.NoContent)
            : BrokerResponse<CustomersOrderResponse>.Success(order, response.StatusCode);
    }

    private static string? ExtractErrorMessage(string? rawError)
    {
        if (string.IsNullOrWhiteSpace(rawError)) return null;

        var trimmed = rawError.Trim();

        try
        {
            using var document = JsonDocument.Parse(trimmed);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return trimmed;

            foreach (var propertyName in new[] { "detail", "title", "message" })
            {
                if (!document.RootElement.TryGetProperty(propertyName, out var property)) continue;
                if (property.ValueKind != JsonValueKind.String) continue;

                var value = property.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value.Trim();
            }
        }
        catch (JsonException)
        {
            // Payload is plain text; return as-is.
        }

        return trimmed;
    }
}