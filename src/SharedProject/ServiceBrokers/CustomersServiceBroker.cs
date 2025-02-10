using System.Text.Json;
using Shared.Models;

namespace Shared.ServiceBrokers;

public class CustomersServiceBroker : ICustomersServiceBroker
{
    private readonly HttpClient _httpClient;

    public CustomersServiceBroker(IConfiguration configuration)
    {
        var serviceUrl = configuration["Services:Customers:Url"] ??
                         throw new ArgumentNullException(nameof(configuration));
        _httpClient = new HttpClient { BaseAddress = new Uri(serviceUrl) };
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