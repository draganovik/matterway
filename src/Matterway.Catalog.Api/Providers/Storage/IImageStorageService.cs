namespace Matterway.Catalog.Api.Providers.Storage;

public interface IImageStorageService
{
    Task<ImageStorageUploadResult> UploadAsync(Guid productId, Guid imageId, IFormFile file,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid productId, Guid imageId, CancellationToken cancellationToken = default);
}

public sealed record ImageStorageUploadResult(Guid ImageId, string ImageUrl);