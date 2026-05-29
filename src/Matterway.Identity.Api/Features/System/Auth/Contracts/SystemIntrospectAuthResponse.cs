namespace Matterway.Identity.Api.Features.System.Auth.Contracts;

public class SystemIntrospectAuthResponse
{
    public List<ClaimResponse> Claims { get; set; } = new();

    public class ClaimResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}