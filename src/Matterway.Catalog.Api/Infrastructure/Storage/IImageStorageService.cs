namespace Matterway.Catalog.Api.Infrastructure.Storage;

public interface IImageStorageService
{
    Task<string> UploadAsync(Guid imageId, IFormFile file,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid imageId, CancellationToken cancellationToken = default);
}