using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Matterway.Catalog.Api.Infrastructure.Storage.Minio;

public sealed class ImageStorageInitializer(
    IMinioClientFactory clientFactory,
    IOptions<ImageStorageOptions> options,
    ILogger<ImageStorageInitializer> logger)
    : IHostedService
{
    private readonly ImageStorageOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var client = clientFactory.CreateClient();
            await EnsureBucketAsync(client, cancellationToken).ConfigureAwait(false);
            await EnsureBucketReadPolicyAsync(client, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize image storage bucket '{Bucket}'.", _options.Bucket);
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task EnsureBucketAsync(IMinioClient client, CancellationToken cancellationToken)
    {
        var bucketExists = await client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.Bucket),
            cancellationToken).ConfigureAwait(false);

        if (bucketExists) return;

        logger.LogInformation("Creating image storage bucket '{Bucket}'.", _options.Bucket);
        await client.MakeBucketAsync(
            new MakeBucketArgs().WithBucket(_options.Bucket),
            cancellationToken).ConfigureAwait(false);
    }

    private async Task EnsureBucketReadPolicyAsync(IMinioClient client, CancellationToken cancellationToken)
    {
        var policyJson = $$"""
                           {
                             "Version": "2012-10-17",
                             "Statement": [
                               {
                                 "Effect": "Allow",
                                 "Principal": {
                                   "AWS": [
                                     "*"
                                   ]
                                 },
                                 "Action": [
                                   "s3:GetObject"
                                 ],
                                 "Resource": [
                                   "arn:aws:s3:::{{_options.Bucket}}/images/*"
                                 ]
                               }
                             ]
                           }
                           """;

        logger.LogInformation(
            "Ensuring anonymous read policy for image storage bucket '{Bucket}' on prefix 'images/*'.",
            _options.Bucket);

        await client.SetPolicyAsync(
            new SetPolicyArgs()
                .WithBucket(_options.Bucket)
                .WithPolicy(policyJson),
            cancellationToken).ConfigureAwait(false);
    }
}