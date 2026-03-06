using System.Net;
using System.Text.Json;

namespace Matterway.Customers.Api.Infrastructure.Brokers.Catalog;

public class HttpCatalogClient(HttpClient httpClient) : ICatalogClient
{
    public async Task<BrokerResponse<CatalogClientGetArticleResponse>> GetArticleByCode(ArticleCode code,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/public/v1/articles/{code}");
        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            return BrokerResponse<CatalogClientGetArticleResponse>.Failure(
                string.IsNullOrWhiteSpace(error) ? response.ReasonPhrase : error,
                response.StatusCode);
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var payload = await JsonSerializer.DeserializeAsync<CatalogClientGetArticleResponse>(contentStream, options,
            cancellationToken);

        return payload is null
            ? BrokerResponse<CatalogClientGetArticleResponse>.Failure("Empty response from catalog.",
                HttpStatusCode.NoContent)
            : BrokerResponse<CatalogClientGetArticleResponse>.Success(payload, response.StatusCode);
    }
}