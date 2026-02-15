using System.Net;

namespace Matterway.Customers.Api.Infrastructure.Brokers.Identity;

public class HttpIdentityClient(HttpClient httpClient) : IIdentityClient
{
    private const string CreateCustomerUserPath = "/api/system/v1.0/users/customer";

    public async Task<BrokerResponse<CreateCustomerUserResponse>> CreateCustomerUserAsync(
        CreateCustomerUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(CreateCustomerUserPath, request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var detail = await response.Content.ReadAsStringAsync(cancellationToken);
            return BrokerResponse<CreateCustomerUserResponse>.Failure(
                string.IsNullOrWhiteSpace(detail) ? response.ReasonPhrase : detail,
                response.StatusCode);
        }

        var payload =
            await response.Content.ReadFromJsonAsync<CreateCustomerUserResponse>(cancellationToken);
        if (payload is null)
            return BrokerResponse<CreateCustomerUserResponse>.Failure("Empty response from identity.",
                HttpStatusCode.NoContent);

        return BrokerResponse<CreateCustomerUserResponse>.Success(payload, response.StatusCode);
    }
}