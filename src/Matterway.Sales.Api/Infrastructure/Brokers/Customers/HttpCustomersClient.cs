using System.Net.Http.Headers;
using System.Text.Json;
using System.Net;
using Matterway.ServiceDefaults.Api;

namespace Matterway.Sales.Api.Infrastructure.Brokers.Customers;

public class HttpCustomersClient(HttpClient httpClient) : ICustomersClient
{
    public async Task<BrokerResponse<CustomersOrderResponse>> CreateOrderAsync(Guid customerId,
        CustomersCreateOrderRequest request,
        string? authorizationHeader,
        CancellationToken cancellationToken)
    {
        var httpRequest =
            new HttpRequestMessage(HttpMethod.Post, "/api/self/v1.0/orders")
            {
                Content = JsonContent.Create(request)
            };

        if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
            AuthenticationHeaderValue.TryParse(authorizationHeader, out var authHeader))
            httpRequest.Headers.Authorization = authHeader;

        var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            return BrokerResponse<CustomersOrderResponse>.Failure(
                string.IsNullOrWhiteSpace(error) ? response.ReasonPhrase : error,
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
}