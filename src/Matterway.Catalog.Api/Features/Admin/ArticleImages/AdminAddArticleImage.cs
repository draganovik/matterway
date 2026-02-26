using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;
using Matterway.Catalog.Api.Infrastructure.Storage;

namespace Matterway.Catalog.Api.Features.Admin.ArticleImages;

public class AdminAddArticleImage : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "articles/{articleId:guid}/images", Handle)
            .WithName("AdminAddArticleImage").WithSummary("[admin] Add a new ArticleImage")
            .WithTags(nameof(ArticleImage))
            .Produces<AddArticleImageResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Accepts<AddArticleImageRequest>("multipart/form-data")
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<AddArticleImageResponse>, BadRequest<ProblemDetails>>> Handle(
        [FromRoute]
        Guid articleId,
        [FromForm]
        AddArticleImageRequest request,
        HttpContext httpContext,
        IArticleImageRepository articleImageRepository,
        IImageStorageService imageStorageService,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Image file is required."
            });

        var imageId = Guid.CreateVersion7();
        ImageStorageUploadResult uploadResult;
        try
        {
            uploadResult =
                await imageStorageService.UploadAsync(articleId, imageId, request.File, cancellationToken);
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

        var entity = MapToEntity(articleId, request, uploadResult);
        ArticleImage? created;

        try
        {
            created = await articleImageRepository.Create(entity, cancellationToken);
        }
        catch
        {
            await imageStorageService.DeleteAsync(articleId, imageId, cancellationToken);
            throw;
        }

        var articleImageModel = MapToResponse(created);

        if (articleImageModel is null)
        {
            await imageStorageService.DeleteAsync(articleId, imageId, cancellationToken);
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        return TypedResults.Created(articleImageModel.ImageUrl,
            articleImageModel);
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
        public Guid ArticleId { get; init; }
        public string? ArticleName { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
    }

    public static ArticleImage MapToEntity(Guid articleId, AddArticleImageRequest request,
        ImageStorageUploadResult uploadResult)
    {
        return new ArticleImage
        {
            Id = uploadResult.ImageId,
            ArticleId = articleId,
            OrderIndex = request.OrderIndex,
            ImageUrl = uploadResult.ImageUrl,
            ImageAlt = request.ImageAlt ?? string.Empty
        };
    }

    public static AddArticleImageResponse? MapToResponse(ArticleImage? entity)
    {
        return entity is null
            ? null
            : new AddArticleImageResponse
            {
                Id = entity.Id,
                OrderIndex = entity.OrderIndex,
                ArticleId = entity.ArticleId,
                ArticleName = entity.Article?.Title,
                ImageUrl = entity.ImageUrl,
                ImageAlt = entity.ImageAlt
            };
    }
}