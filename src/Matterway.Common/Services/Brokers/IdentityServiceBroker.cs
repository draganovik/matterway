using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Matterway.Common.Services.Brokers;

public class IdentityServiceBroker : IIdentityServiceBroker
{
    private readonly HttpClient _httpClient;

    public IdentityServiceBroker(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        // call an Auth introspection endpoint on Matterway.Identity.Api to check if Token is valid
        // token should be passed in as Bearer token
        // if token is valid, user claims should be returned
        // if token is invalid, Unauthorized should be thrown
        // if token is expired, Unauthorized should be thrown
        // if token is not found, Unauthorized should be thrown
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1.0/Auth/Introspect");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var payload = JsonSerializer.Deserialize<AuthIntrospectResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (payload?.Claims is null || payload.Claims.Count == 0) return null;

            var claimsIdentity = new ClaimsIdentity(
                payload.Claims.Select(claim => new Claim(claim.Type, claim.Value)),
                "Token");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return claimsPrincipal;
        }

        return null;
    }
}