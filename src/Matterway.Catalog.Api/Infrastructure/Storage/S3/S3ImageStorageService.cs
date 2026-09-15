using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace Matterway.Catalog.Api.Infrastructure.Storage.S3;

public sealed class S3ImageStorageService(
    IAmazonS3 client,
    IOptions<ImageStorageOptions> options)
    : IImageStorageService
{
    private readonly ImageStorageOptions _options = options.Value;

    public async Task<string> UploadAsync(Guid imageId, IFormFile file,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        if (file.Length == 0) throw new InvalidOperationException("Cannot upload an empty file.");

        await using var stream = file.OpenReadStream();
        await client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _options.Bucket,
            Key = ImageStoragePaths.BuildObjectName(imageId),
            InputStream = stream,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
            AutoCloseStream = false,
            UseChunkEncoding = false
        }, cancellationToken).ConfigureAwait(false);

        return ImageStoragePaths.BuildStorageUrl(_options.Endpoint, _options.Bucket, imageId);
    }

    public async Task DeleteAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        // S3 DeleteObject also succeeds when the key does not exist.
        await client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _options.Bucket,
            Key = ImageStoragePaths.BuildObjectName(imageId)
        }, cancellationToken).ConfigureAwait(false);
    }
}
