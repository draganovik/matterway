using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;
using Matterway.Catalog.Api.Infrastructure.Storage;

namespace Matterway.Catalog.Api.Endpoints.Admin.ArticleImages;

public class AdminAddArticleImage : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "articles/{article:ArticleCode}/images", Handle)
            .WithName("AdminAddArticleImage").WithSummary("[admin] Add a new ArticleImage")
            .WithTags(nameof(ArticleImage))
            .Produces<AddArticleImageResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Accepts<AddArticleImageRequest>("multipart/form-data")
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<AddArticleImageResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        [FromRoute]
        ArticleCode article,
        [FromForm]
        AddArticleImageRequest request,
        HttpContext httpContext,
        IArticleRepository articleRepository,
        IArticleImageRepository articleImageRepository,
        IImageStorageService imageStorageService,
        CancellationToken cancellationToken)
    {
        var articleEntity = await articleRepository.GetBy(article, cancellationToken);
        if (articleEntity is null) return TypedResults.NotFound();

        if (request.File is null || request.File.Length == 0)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Image file is required."
            });

        var imageId = Guid.CreateVersion7();
        try
        {
            await imageStorageService.UploadAsync(imageId, request.File, cancellationToken);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Upload failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
        }

        var entity = new ArticleImage
        {
            Id = imageId,
            ArticleCode = article.ToString(),
            OrderIndex = request.OrderIndex,
            ImageUrl = CatalogImagePaths.BuildPublicUrl(imageId),
            ImageAlt = request.ImageAlt ?? string.Empty
        };
        ArticleImage? created;

        try
        {
            created = await articleImageRepository.Create(entity, cancellationToken);
        }
        catch
        {
            await imageStorageService.DeleteAsync(imageId, cancellationToken);
            throw;
        }

        if (created is null)
        {
            await imageStorageService.DeleteAsync(imageId, cancellationToken);
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var articleImageModel = new AddArticleImageResponse
        {
            Id = created.Id,
            OrderIndex = created.OrderIndex,
            ArticleCode = article,
            ArticleName = articleEntity.Title,
            ImageUrl = CatalogImagePaths.BuildPublicUrl(created.Id),
            ImageAlt = created.ImageAlt
        };

        return TypedResults.Created(articleImageModel.ImageUrl, articleImageModel);
    }

    public record AddArticleImageRequest
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int OrderIndex { get; init; }

        [Required]
        public IFormFile? File { get; init; }

        public string? ImageAlt { get; init; }
    }

    public record AddArticleImageResponse
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public required ArticleCode ArticleCode { get; init; }
        public string? ArticleName { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
    }
}