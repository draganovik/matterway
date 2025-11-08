using Matterway.Catalog.Api.Infrastructure.Storage;
using Matterway.Catalog.Api.Infrastructure.Storage.Minio;

namespace Matterway.Catalog.Api.Extensions;

public static class ImageStorageExtensions
{
    public static void ConfigureImageStorage(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<ImageStorageOptions>()
            .BindConfiguration(ImageStorageOptions.SectionName)
            .ValidateDataAnnotations()
            .Validate(options => !string.IsNullOrWhiteSpace(options.Bucket), "Image storage bucket must be provided.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Endpoint), "Image storage endpoint must be provided.")
            .ValidateOnStart();

        builder.Services.AddSingleton<IImageStorageService, MinioImageStorageService>();
    }
}
