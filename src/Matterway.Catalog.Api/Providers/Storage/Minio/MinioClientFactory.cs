using Minio;
using Microsoft.Extensions.Options;

namespace Matterway.Catalog.Api.Providers.Storage.Minio;

public sealed class MinioClientFactory(IOptions<ImageStorageOptions> options) : IMinioClientFactory
{
    private readonly ImageStorageOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));

    public IMinioClient CreateClient()
    {
        var endpoint = ParseEndpoint(_options.Endpoint);

        var clientBuilder = new MinioClient()
            .WithEndpoint(endpoint.Host, endpoint.Port)
            .WithCredentials(_options.AccessKey, _options.SecretKey);

        if (endpoint.UseSsl) clientBuilder = clientBuilder.WithSSL();

        if (!string.IsNullOrWhiteSpace(_options.Region)) clientBuilder = clientBuilder.WithRegion(_options.Region);

        return clientBuilder.Build();
    }

    private static (string Host, int Port, bool UseSsl) ParseEndpoint(string endpoint)
    {
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
            throw new ArgumentException($"Invalid storage endpoint '{endpoint}'.", nameof(endpoint));

        var port = uri.Port;
        if (port == -1)
            port = string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ? 443 : 80;

        return (uri.Host, port, string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase));
    }
}