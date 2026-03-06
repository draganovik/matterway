namespace Matterway.Catalog.Api.Infrastructure.Storage;

public interface IImageStorageService
{
    Task<ImageStorageUploadResult> UploadAsync(Guid imageId, IFormFile file,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid imageId, CancellationToken cancellationToken = default);
}

public sealed record ImageStorageUploadResult(Guid ImageId, string ImageUrl);