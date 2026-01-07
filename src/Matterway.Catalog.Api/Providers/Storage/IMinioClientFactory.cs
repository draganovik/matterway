using Minio;

namespace Matterway.Catalog.Api.Providers.Storage;

public interface IMinioClientFactory
{
    IMinioClient CreateClient();
}