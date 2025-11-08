using Microsoft.AspNetCore.Http;

namespace Matterway.Catalog.Api.Infrastructure.Storage;

public interface IImageStorageService
{
    Task<ImageStorageUploadResult> UploadAsync(Guid productId, IFormFile file, CancellationToken cancellationToken = default);

    Task DeleteAsync(string imageRef, CancellationToken cancellationToken = default);
}

public sealed record ImageStorageUploadResult(string ImageRef, string ImageUrl);
