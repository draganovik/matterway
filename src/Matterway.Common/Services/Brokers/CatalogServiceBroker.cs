using Matterway.Common.Models;
using System.Text.Json;

namespace Matterway.Common.Services.Brokers;

public class CatalogServiceBroker : ICatalogServiceBroker
{
    private readonly HttpClient _httpClient;

    public CatalogServiceBroker(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<Product?> GetProductById(Guid id)
    {
        // call a Get endpoint on Matterway.Catalog.Api to get Product by Id
        // if Product is found, return it
        // if Product is not found, return null
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1.0/Products/{id}");
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            // Get the Product from the resposne return Product
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<Product>(content, options);
        }

        return null;
    }
}