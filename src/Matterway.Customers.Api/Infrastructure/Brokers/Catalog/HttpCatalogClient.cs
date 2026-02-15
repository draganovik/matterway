using System.Net;
using System.Text.Json;

namespace Matterway.Customers.Api.Infrastructure.Brokers.Catalog;

public class HttpCatalogClient(HttpClient httpClient) : ICatalogClient
{
    public async Task<BrokerResponse<CatalogClientGetArticleByIdResponse>> GetArticleById(Guid id,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/public/v1.0/articles/{id}");
        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            return BrokerResponse<CatalogClientGetArticleByIdResponse>.Failure(
                string.IsNullOrWhiteSpace(error) ? response.ReasonPhrase : error,
                response.StatusCode);
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var payload = await JsonSerializer.DeserializeAsync<CatalogClientGetArticleByIdResponse>(contentStream, options,
            cancellationToken);

        return payload is null
            ? BrokerResponse<CatalogClientGetArticleByIdResponse>.Failure("Empty response from catalog.",
                HttpStatusCode.NoContent)
            : BrokerResponse<CatalogClientGetArticleByIdResponse>.Success(payload, response.StatusCode);
    }
}