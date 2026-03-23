using Microsoft.Extensions.Options;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Matterway.Catalog.Api.Infrastructure.Storage.Minio;

public sealed class MinioImageStorageService(
    IMinioClientFactory clientFactory,
    IOptions<ImageStorageOptions> options)
    : IImageStorageService
{
    private readonly ImageStorageOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));

    public async Task<string> UploadAsync(Guid imageId, IFormFile file,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Length == 0) throw new InvalidOperationException("Cannot upload an empty file.");

        var objectName = ImageStoragePaths.BuildObjectName(imageId);

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

        return ImageStoragePaths.BuildStorageUrl(_options.Endpoint, _options.Bucket, imageId);
    }

    public async Task DeleteAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var objectName = ImageStoragePaths.BuildObjectName(imageId);
        var client = clientFactory.CreateClient();
        var removeArgs = new RemoveObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectName);

        try
        {
            await client.RemoveObjectAsync(removeArgs, cancellationToken).ConfigureAwait(false);
        }
        catch (ObjectNotFoundException)
        {
            // Ignore missing objects
        }
    }
}