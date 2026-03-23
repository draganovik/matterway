using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Endpoints.Public.Articles;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;
using Matterway.Catalog.Api.Infrastructure.Storage;

namespace Matterway.Catalog.Api.Endpoints.Admin.ArticleImages;

public class AdminAddArticleImage : IEndpoint
{
    private const string RouteName = nameof(AdminAddArticleImage);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "articles/{article:ArticleCode}/images", Handle)
            .WithName(RouteName).WithSummary("[admin] Add a new ArticleImage")
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
        if (await articleRepository.GetBy(article, cancellationToken) is null) return TypedResults.NotFound();

        if (request.File is null || request.File.Length == 0)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Image file is required."
            });

        var imageId = Guid.CreateVersion7();
        string imageUrl;
        try
        {
            imageUrl = await imageStorageService.UploadAsync(imageId, request.File, cancellationToken);
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
            ImageUrl = imageUrl,
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

        var refreshedImages = await articleImageRepository.GetByArticle(article, cancellationToken);
        var articleImageModel = new AddArticleImageResponse
        {
            Images = refreshedImages
                .Select(PublicGetArticleByCode.MapImageToResponse)
                .ToList(),
            CreatedImageId = created.Id
        };

        var location = $"{httpContext.Request.PathBase}{httpContext.Request.Path}/{created.OrderIndex}";
        return TypedResults.Created(location, articleImageModel);
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
        public ICollection<PublicGetArticleByCode.ArticleImageProperty>? Images { get; init; } = [];
        public required Guid CreatedImageId { get; init; }
    }
}