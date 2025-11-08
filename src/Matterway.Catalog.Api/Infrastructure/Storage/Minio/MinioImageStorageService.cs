using Minio.DataModel.Args;
using Minio.Exceptions;
using Microsoft.Extensions.Options;

namespace Matterway.Catalog.Api.Infrastructure.Storage.Minio;

public sealed class MinioImageStorageService(
    IMinioClientFactory clientFactory,
    IOptions<ImageStorageOptions> options)
    : IImageStorageService
{
    private readonly ImageStorageOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));

    public async Task<ImageStorageUploadResult> UploadAsync(Guid productId, IFormFile file,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Length == 0)
        {
            throw new InvalidOperationException("Cannot upload an empty file.");
        }

        var objectName = BuildObjectName(productId, file.FileName);

        await using var stream = file.OpenReadStream();
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(
                string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);

        var client = clientFactory.CreateClient();
        await client.PutObjectAsync(putObjectArgs, cancellationToken).ConfigureAwait(false);

        return new ImageStorageUploadResult(objectName, BuildPublicUrl(objectName));
    }

    public async Task DeleteAsync(string imageRef, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imageRef))
        {
            return;
        }

        var client = clientFactory.CreateClient();
        var removeArgs = new RemoveObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(imageRef);

        try
        {
            await client.RemoveObjectAsync(removeArgs, cancellationToken).ConfigureAwait(false);
        }
        catch (ObjectNotFoundException)
        {
            // Ignore missing objects
        }
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
}