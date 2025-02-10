using System.Text.Json;
using Shared.Models;

namespace Shared.ServiceBrokers;

public class CatalogServiceBroker : ICatalogServiceBroker
{
    private readonly HttpClient _httpClient;

    public CatalogServiceBroker(IConfiguration configuration)
    {
        var serviceUrl = configuration["Services:Catalog:Url"] ??
                         throw new ArgumentNullException(nameof(configuration));
        _httpClient = new HttpClient { BaseAddress = new Uri(serviceUrl) };
    }

    public async Task<Product?> GetProductById(Guid id)
    {
        // call a Get endpoint on Catalog.API to get Product by Id
        // if Product is found, return it
        // if Product is not found, return null
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Products/{id}");
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