using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace Matterway.Catalog.Api.Infrastructure.Storage.S3;

public sealed class ImageStorageInitializer(
    IAmazonS3 client,
    IOptions<ImageStorageOptions> options,
    ILogger<ImageStorageInitializer> logger)
    : IHostedService
{
    private readonly ImageStorageOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
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

    private async Task EnsureBucketAsync(IAmazonS3 client, CancellationToken cancellationToken)
    {
        try
        {
            await client.HeadBucketAsync(new HeadBucketRequest
            {
                BucketName = _options.Bucket
            }, cancellationToken).ConfigureAwait(false);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogInformation("Creating image storage bucket '{Bucket}'.", _options.Bucket);
            try
            {
                await client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = _options.Bucket
                }, cancellationToken).ConfigureAwait(false);
            }
            catch (AmazonS3Exception createException) when (createException.ErrorCode == "BucketAlreadyOwnedByYou")
            {
                // Another Catalog instance created the bucket concurrently.
            }
        }
    }

    private async Task EnsureBucketReadPolicyAsync(IAmazonS3 client, CancellationToken cancellationToken)
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

        await client.PutBucketPolicyAsync(
            new PutBucketPolicyRequest
            {
                BucketName = _options.Bucket,
                Policy = policyJson
            },
            cancellationToken).ConfigureAwait(false);
    }
}