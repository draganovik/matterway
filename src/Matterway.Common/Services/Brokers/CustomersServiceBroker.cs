using System.Text.Json;
using Matterway.Common.Models;

namespace Matterway.Common.Services.Brokers;

public class CustomersServiceBroker : ICustomersServiceBroker
{
    private readonly HttpClient _httpClient;

    public CustomersServiceBroker(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<Guid?> VerifyBySystemUserId(Guid systemUserId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Customers/VerifyBy?systemUserId={systemUserId}");
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var customer = JsonSerializer.Deserialize<Customer>(content, options);
            if (customer != null) return customer.Id;
        }

        return null;
    }

    public async Task<bool> VerifyByCustomerId(Guid customerId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Customers/VerifyBy?customerId={customerId}");
        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}