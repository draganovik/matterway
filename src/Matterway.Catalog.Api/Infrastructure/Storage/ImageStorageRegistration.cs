using Matterway.Catalog.Api.Infrastructure.Storage.Minio;

namespace Matterway.Catalog.Api.Infrastructure.Storage;

public static class ImageStorageRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureImageStorage()
        {
            builder.Services.AddOptions<ImageStorageOptions>()
                .BindConfiguration(ImageStorageOptions.SectionName)
                .ValidateDataAnnotations()
                .Validate(options => !string.IsNullOrWhiteSpace(options.Bucket),
                    "Image storage bucket must be provided.")
                .Validate(options => !string.IsNullOrWhiteSpace(options.Endpoint),
                    "Image storage endpoint must be provided.")
                .Validate(options => !string.IsNullOrWhiteSpace(options.AccessKey),
                    "Image storage access key must be provided.")
                .Validate(options => !string.IsNullOrWhiteSpace(options.SecretKey),
                    "Image storage secret key must be provided.")
                .ValidateOnStart();

            builder.Services.AddSingleton<IMinioClientFactory, MinioClientFactory>();
            builder.Services.AddScoped<IImageStorageService, MinioImageStorageService>();
            builder.Services.AddHostedService<ImageStorageInitializer>();

            return builder;
        }
    }
}