namespace Matterway.ServiceDefaults.Authorization;

public sealed record SystemAccessKeyOptions
{
    public const string ConfigurationPath = "Security:SystemAccessKey";
    public const string HeaderName = "X-System-Access-Key";
    public required string AccessKey { get; init; }
    public required IReadOnlySet<string> AllowedAuthorities { get; init; }
}