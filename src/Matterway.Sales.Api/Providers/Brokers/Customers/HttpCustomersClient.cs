using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Matterway.Sales.Api.Providers.Brokers.Customers;

public class HttpCustomersClient(HttpClient httpClient) : ICustomersClient
{
    public async Task<CustomersOrderResult> CreateOrderAsync(Guid customerId, CustomersCreateOrderRequest request,
        string? authorizationHeader,
        CancellationToken cancellationToken)
    {
        var httpRequest =
            new HttpRequestMessage(HttpMethod.Post, $"/api/v1.0/Customers/{customerId}/Orders")
            {
                Content = JsonContent.Create(request)
            };

        if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
            AuthenticationHeaderValue.TryParse(authorizationHeader, out var authHeader))
            httpRequest.Headers.Authorization = authHeader;

        var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return new CustomersOrderResult(response.StatusCode, null);

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var order = await JsonSerializer.DeserializeAsync<CustomersOrderResponse>(contentStream, options,
            cancellationToken);

        return new CustomersOrderResult(response.StatusCode, order);
    }
}