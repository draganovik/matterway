using Amazon.Runtime;
using Amazon.S3;

namespace Matterway.Catalog.Api.Infrastructure.Storage.S3;

public static class S3Client
{
    public static IAmazonS3 Create(ImageStorageOptions options)
    {
        if (!Uri.TryCreate(options.Endpoint, UriKind.Absolute, out var endpoint) ||
            (endpoint.Scheme != Uri.UriSchemeHttp && endpoint.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Storage endpoint must be an absolute HTTP or HTTPS URL.", nameof(options));

        return new AmazonS3Client(new BasicAWSCredentials(options.AccessKey, options.SecretKey),
            new AmazonS3Config
            {
                ServiceURL = endpoint.GetLeftPart(UriPartial.Authority),
                ForcePathStyle = true,
                AuthenticationRegion = string.IsNullOrWhiteSpace(options.Region) ? "us-east-1" : options.Region,
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            });
    }
}
