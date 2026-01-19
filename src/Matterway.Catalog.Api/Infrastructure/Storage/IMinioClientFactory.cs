using Minio;

namespace Matterway.Catalog.Api.Infrastructure.Storage;

public interface IMinioClientFactory
{
    IMinioClient CreateClient();
}