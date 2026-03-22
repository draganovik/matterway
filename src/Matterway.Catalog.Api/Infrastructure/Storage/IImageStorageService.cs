namespace Matterway.Catalog.Api.Infrastructure.Storage;

public interface IImageStorageService
{
    Task UploadAsync(Guid imageId, IFormFile file,
        CancellationToken cancellationToken = default);

    Task<ImageStorageDownloadResult?> DownloadAsync(Guid imageId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid imageId, CancellationToken cancellationToken = default);
}

public sealed record ImageStorageDownloadResult(byte[] Content, string ContentType);