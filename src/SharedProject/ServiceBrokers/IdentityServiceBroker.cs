using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Shared.ServiceBrokers
{
    public class IdentityServiceBroker : IIdentityServiceBroker
    {
        private readonly HttpClient _httpClient;
        public IdentityServiceBroker(IConfiguration configuration)
        {
            var serviceUrl = configuration["Services:Identity:Url"] ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient = new HttpClient { BaseAddress = new Uri(serviceUrl) };
        }

        public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
        {
            // call an Introspect endpoint on Indendtity.API to check if Token is valid
            // token should be passed in as Bearer token
            // if token is valid, user claims should be returned
            // if token is invalid, Unauthorized should be thrown
            // if token is expired, Unauthorized should be thrown
            // if token is not found, Unauthorized should be thrown
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Sessions/introspect");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
                if (claims == null) return null;
                var claimsIdentity = new ClaimsIdentity(claims.Select(x => new Claim(x.Key, x.Value)), "Token");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                return claimsPrincipal;
            }
            return null;
        }
    }
}
