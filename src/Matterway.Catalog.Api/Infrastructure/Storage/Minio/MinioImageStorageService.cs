using System.IO;
using Minio;
using Minio.DataModel.Args;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Matterway.Catalog.Api.Infrastructure.Storage.Minio;

public sealed class MinioImageStorageService : IImageStorageService
{
    private readonly ImageStorageOptions _options;
    private readonly IMinioClient _client;
    private readonly SemaphoreSlim _bucketSemaphore = new(1, 1);
    private bool _bucketReady;
    private bool _publicAccessConfigured;

    public MinioImageStorageService(IOptions<ImageStorageOptions> options)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        var endpoint = ParseEndpoint(_options.Endpoint);

        var clientBuilder = new MinioClient()
            .WithEndpoint(endpoint.Host, endpoint.Port)
            .WithCredentials(_options.AccessKey, _options.SecretKey);

        if (endpoint.UseSsl)
        {
            clientBuilder = clientBuilder.WithSSL();
        }

        if (!string.IsNullOrWhiteSpace(_options.Region))
        {
            clientBuilder = clientBuilder.WithRegion(_options.Region);
        }

        _client = clientBuilder.Build();
    }

    public async Task<ImageStorageUploadResult> UploadAsync(Guid productId, IFormFile file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Length == 0)
        {
            throw new InvalidOperationException("Cannot upload an empty file.");
        }

        await EnsureBucketAsync(cancellationToken).ConfigureAwait(false);

        var objectName = BuildObjectName(productId, file.FileName);

        await using var stream = file.OpenReadStream();
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);

        await _client.PutObjectAsync(putObjectArgs, cancellationToken).ConfigureAwait(false);

        return new ImageStorageUploadResult(objectName, BuildPublicUrl(objectName));
    }

    public async Task DeleteAsync(string imageRef, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imageRef))
        {
            return;
        }

        if (!_bucketReady)
        {
            await EnsureBucketAsync(cancellationToken).ConfigureAwait(false);
        }

        var removeArgs = new RemoveObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(imageRef);

        await _client.RemoveObjectAsync(removeArgs, cancellationToken).ConfigureAwait(false);
    }

    private async Task EnsureBucketAsync(CancellationToken cancellationToken)
    {
        if (_bucketReady)
        {
            return;
        }

        await _bucketSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_bucketReady)
            {
                return;
            }

            var bucketExists = await _client.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_options.Bucket),
                cancellationToken).ConfigureAwait(false);

            if (!bucketExists)
            {
                await _client.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(_options.Bucket),
                    cancellationToken).ConfigureAwait(false);
            }

            _bucketReady = true;
        }
        finally
        {
            _bucketSemaphore.Release();
        }

        if (_options.AllowPublicRead && !_publicAccessConfigured)
        {
            await EnsurePublicReadPolicyAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private static (string Host, int Port, bool UseSsl) ParseEndpoint(string endpoint)
    {
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException($"Invalid storage endpoint '{endpoint}'.", nameof(endpoint));
        }

        var port = uri.Port;
        if (port == -1)
        {
            port = string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ? 443 : 80;
        }

        return (uri.Host, port, string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase));
    }

    private static string BuildObjectName(Guid productId, string? originalFileName)
    {
        var extension = Path.GetExtension(originalFileName ?? string.Empty);
        var sanitizedExtension = string.IsNullOrWhiteSpace(extension)
            ? ".bin"
            : extension.ToLowerInvariant();

        return $"{productId:D}/{Guid.CreateVersion7():N}{sanitizedExtension}";
    }

    private string BuildPublicUrl(string objectName)
    {
        var baseUrl = string.IsNullOrWhiteSpace(_options.PublicBaseUrl)
            ? _options.Endpoint
            : _options.PublicBaseUrl!;

        return $"{baseUrl.TrimEnd('/')}/{_options.Bucket}/{objectName}";
    }

    private async Task EnsurePublicReadPolicyAsync(CancellationToken cancellationToken)
    {
        var policy =
            $$"""
              {
                "Version": "2012-10-17",
                "Statement": [
                  {
                    "Effect": "Allow",
                    "Principal": {"AWS": "*"},
                    "Action": ["s3:GetObject"],
                    "Resource": ["arn:aws:s3:::{{_options.Bucket}}/*"]
                  }
                ]
              }
              """;

        await _client.SetPolicyAsync(
            new SetPolicyArgs()
                .WithBucket(_options.Bucket)
                .WithPolicy(policy),
            cancellationToken).ConfigureAwait(false);

        _publicAccessConfigured = true;
    }
}
