using System.IO.Compression;
using System.Text.Json;
using Matterway.Catalog.Api.Infrastructure.ImportExport.Rows;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Matterway.Catalog.Api.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio.DataModel.Args;

namespace Matterway.Catalog.Api.Infrastructure.ImportExport;

public sealed class CatalogArchiveService(
    CatalogDbComposer context,
    IMinioClientFactory minioClientFactory,
    IOptions<ImageStorageOptions> options,
    ILogger<CatalogArchiveService> logger)
    : ICatalogArchiveService
{
    private const string TruncateSql = """
                                       TRUNCATE TABLE
                                           "ArticleDetailNumeric",
                                           "ArticleDetailText",
                                           "ArticleImage",
                                           "Discount",
                                           "Article",
                                           "Detail"
                                       CASCADE;
                                       """;

    private readonly ImageStorageOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public async Task<ExportCatalogArchiveResult> ExportAsync(CancellationToken cancellationToken = default)
    {
        var details = (await context.Detail
                .AsNoTracking()
                .OrderBy(x => x.Slug)
                .ToListAsync(cancellationToken))
            .Select(DetailRow.FromEntity)
            .ToList();

        var articles = (await context.Article
                .AsNoTracking()
                .OrderBy(x => x.ArticleCode)
                .ToListAsync(cancellationToken))
            .Select(ArticleRow.FromEntity)
            .ToList();

        var discounts = (await context.Discount
                .AsNoTracking()
                .OrderBy(x => x.Code)
                .ThenBy(x => x.ArticleCode)
                .ToListAsync(cancellationToken))
            .Select(DiscountRow.FromEntity)
            .ToList();

        var articleDetailTexts = (await context.ArticleDetailText
                .AsNoTracking()
                .OrderBy(x => x.ArticleCode)
                .ThenBy(x => x.DetailSlug)
                .ToListAsync(cancellationToken))
            .Select(ArticleDetailTextRow.FromEntity)
            .ToList();

        var articleDetailNumerics = (await context.ArticleDetailNumeric
                .AsNoTracking()
                .OrderBy(x => x.ArticleCode)
                .ThenBy(x => x.DetailSlug)
                .ToListAsync(cancellationToken))
            .Select(ArticleDetailNumericRow.FromEntity)
            .ToList();

        var dbImages = await context.ArticleImage
            .AsNoTracking()
            .OrderBy(x => x.ArticleCode)
            .ThenBy(x => x.OrderIndex)
            .ToListAsync(cancellationToken);

        var imageRows = new List<ArticleImageRow>(dbImages.Count);

        await using var zipStream = new MemoryStream();
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var dbImage in dbImages)
            {
                var objectName = CatalogImagePaths.BuildObjectName(dbImage.Id);
                var (content, contentType) = await LoadImageContentAsync(objectName, cancellationToken);

                var fileExtension = ResolveExtension(contentType);
                var imageFile = $"images/{dbImage.ArticleCode}/{dbImage.Id:N}.{fileExtension}";
                await WriteBinaryEntryAsync(archive, imageFile, content, cancellationToken);

                imageRows.Add(ArticleImageRow.FromEntity(dbImage, imageFile, contentType));
            }

            var manifest = ArchiveManifestRow.Create(
                details.Count,
                articles.Count,
                discounts.Count,
                articleDetailTexts.Count,
                articleDetailNumerics.Count,
                imageRows.Count);

            await WriteJsonEntryAsync(archive, "manifest.json", manifest, cancellationToken);
            await WriteJsonEntryAsync(archive, "data/details.json", details, cancellationToken);
            await WriteJsonEntryAsync(archive, "data/articles.json", articles, cancellationToken);
            await WriteJsonEntryAsync(archive, "data/discounts.json", discounts, cancellationToken);
            await WriteJsonEntryAsync(archive, "data/article_detail_texts.json", articleDetailTexts, cancellationToken);
            await WriteJsonEntryAsync(archive, "data/article_detail_numerics.json", articleDetailNumerics,
                cancellationToken);
            await WriteJsonEntryAsync(archive, "data/article_images.json", imageRows, cancellationToken);
        }

        var fileName = $"catalog-archive-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip";
        return new ExportCatalogArchiveResult(zipStream.ToArray(), fileName, articles.Count, imageRows.Count);
    }

    public async Task<ImportCatalogArchiveResult> ImportAsync(
        Stream archiveStream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(archiveStream);

        await using var buffered = new MemoryStream();
        await archiveStream.CopyToAsync(buffered, cancellationToken);
        buffered.Position = 0;

        using var archive = new ZipArchive(buffered, ZipArchiveMode.Read, false);

        _ = await ReadJsonEntryAsync<ArchiveManifestRow>(archive, "manifest.json", cancellationToken);
        var details = await ReadJsonEntryAsync<List<DetailRow>>(archive, "data/details.json", cancellationToken);
        var articles = await ReadJsonEntryAsync<List<ArticleRow>>(archive, "data/articles.json", cancellationToken);
        var discounts = await ReadJsonEntryAsync<List<DiscountRow>>(archive, "data/discounts.json", cancellationToken);
        var articleDetailTexts = await ReadJsonEntryAsync<List<ArticleDetailTextRow>>(
            archive,
            "data/article_detail_texts.json",
            cancellationToken);
        var articleDetailNumerics = await ReadJsonEntryAsync<List<ArticleDetailNumericRow>>(
            archive,
            "data/article_detail_numerics.json",
            cancellationToken);
        var articleImages = await ReadJsonEntryAsync<List<ArticleImageRow>>(
            archive,
            "data/article_images.json",
            cancellationToken);

        var client = minioClientFactory.CreateClient();
        var uploadedObjectNames = new List<string>(articleImages.Count);

        try
        {
            foreach (var image in articleImages)
            {
                var imageEntry = archive.GetEntry(image.ImageFile);
                if (imageEntry is null)
                    throw new InvalidDataException($"Missing image entry '{image.ImageFile}' in archive.");

                await using var entryStream = await imageEntry.OpenAsync(cancellationToken);
                await using var contentStream = new MemoryStream();
                await entryStream.CopyToAsync(contentStream, cancellationToken);
                contentStream.Position = 0;

                var contentType = string.IsNullOrWhiteSpace(image.ContentType)
                    ? GuessContentType(image.ImageFile)
                    : image.ContentType;

                var objectName = CatalogImagePaths.BuildObjectName(image.Id);
                await client.PutObjectAsync(new PutObjectArgs()
                        .WithBucket(_options.Bucket)
                        .WithObject(objectName)
                        .WithStreamData(contentStream)
                        .WithObjectSize(contentStream.Length)
                        .WithContentType(contentType),
                    cancellationToken);

                uploadedObjectNames.Add(objectName);
            }
        }
        catch
        {
            await RollbackUploadedObjectsAsync(client, uploadedObjectNames, cancellationToken);
            throw;
        }

        var strategy = context.Database.CreateExecutionStrategy();
        try
        {
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    await context.Database.ExecuteSqlRawAsync(TruncateSql, cancellationToken);

                    context.Detail.AddRange(details.Select(x => x.ToEntity()));
                    context.Article.AddRange(articles.Select(x => x.ToEntity()));
                    context.Discount.AddRange(discounts.Select(x => x.ToEntity()));
                    context.ArticleDetailText.AddRange(articleDetailTexts.Select(x => x.ToEntity()));
                    context.ArticleDetailNumeric.AddRange(articleDetailNumerics.Select(x => x.ToEntity()));
                    context.ArticleImage.AddRange(
                        articleImages.Select(x => x.ToEntity(CatalogImagePaths.BuildPublicUrl(x.Id))));

                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
        catch
        {
            await RollbackUploadedObjectsAsync(client, uploadedObjectNames, cancellationToken);
            throw;
        }

        return new ImportCatalogArchiveResult(
            details.Count,
            articles.Count,
            discounts.Count,
            articleDetailTexts.Count,
            articleDetailNumerics.Count,
            articleImages.Count);
    }

    private async Task<(byte[] Content, string ContentType)> LoadImageContentAsync(
        string objectName,
        CancellationToken cancellationToken)
    {
        var client = minioClientFactory.CreateClient();
        try
        {
            var stat = await client.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket(_options.Bucket)
                    .WithObject(objectName),
                cancellationToken);

            await using var buffer = new MemoryStream();
            await client.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(_options.Bucket)
                    .WithObject(objectName)
                    .WithCallbackStream(stream => stream.CopyTo(buffer)),
                cancellationToken);

            return (buffer.ToArray(), string.IsNullOrWhiteSpace(stat.ContentType)
                ? "application/octet-stream"
                : stat.ContentType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Could not read image object '{ObjectName}' from MinIO bucket '{Bucket}'.",
                objectName,
                _options.Bucket);
            throw;
        }
    }

    private async Task RollbackUploadedObjectsAsync(
        Minio.IMinioClient client,
        IReadOnlyCollection<string> objectNames,
        CancellationToken cancellationToken)
    {
        foreach (var objectName in objectNames)
            try
            {
                await client.RemoveObjectAsync(
                    new RemoveObjectArgs()
                        .WithBucket(_options.Bucket)
                        .WithObject(objectName),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "Failed to rollback uploaded object '{ObjectName}' in bucket '{Bucket}'.",
                    objectName,
                    _options.Bucket);
            }
    }

    private static async Task WriteJsonEntryAsync<T>(
        ZipArchive archive,
        string path,
        T payload,
        CancellationToken cancellationToken)
    {
        var entry = archive.CreateEntry(path, CompressionLevel.SmallestSize);
        await using var stream = await entry.OpenAsync(cancellationToken);
        await JsonSerializer.SerializeAsync(stream, payload, Json, cancellationToken);
    }

    private static async Task WriteBinaryEntryAsync(
        ZipArchive archive,
        string path,
        byte[] content,
        CancellationToken cancellationToken)
    {
        var entry = archive.CreateEntry(path, CompressionLevel.SmallestSize);
        await using var stream = await entry.OpenAsync(cancellationToken);
        await stream.WriteAsync(content.AsMemory(), cancellationToken);
    }

    private static async Task<T> ReadJsonEntryAsync<T>(
        ZipArchive archive,
        string path,
        CancellationToken cancellationToken)
    {
        var entry = archive.GetEntry(path);
        if (entry is null) throw new InvalidDataException($"Missing entry '{path}' in archive.");

        await using var stream = await entry.OpenAsync(cancellationToken);
        var payload = await JsonSerializer.DeserializeAsync<T>(stream, Json, cancellationToken);
        if (payload is null) throw new InvalidDataException($"Entry '{path}' has invalid JSON content.");
        return payload;
    }

    private static string ResolveExtension(string contentType)
    {
        var byContentType = contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "image/webp" => "webp",
            "image/gif" => "gif",
            "image/bmp" => "bmp",
            "image/tiff" => "tiff",
            "image/svg+xml" => "svg",
            _ => string.Empty
        };

        if (!string.IsNullOrWhiteSpace(byContentType))
            return byContentType;

        return "bin";
    }

    private static string GuessContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".tiff" or ".tif" => "image/tiff",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }
}