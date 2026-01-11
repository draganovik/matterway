namespace Matterway.Common.Services.Brokers;

public class AuthIntrospectResponse
{
    public List<AuthClaim> Claims { get; set; } = new();
}

public class AuthClaim
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}