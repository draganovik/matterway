using System.Text.Json;

namespace Matterway.Customers.Api.Infrastructure.Brokers.Catalog;

public class HttpCatalogClient(HttpClient httpClient) : ICatalogClient
{
    public async Task<CatalogClientGetProductByIdResponse?> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1.0/Products/{id}");
        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return await JsonSerializer.DeserializeAsync<CatalogClientGetProductByIdResponse>(contentStream, options,
            cancellationToken);
    }
}