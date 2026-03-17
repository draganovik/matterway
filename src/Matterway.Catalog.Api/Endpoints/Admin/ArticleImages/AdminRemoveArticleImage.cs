using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;
using Matterway.Catalog.Api.Infrastructure.Storage;

namespace Matterway.Catalog.Api.Endpoints.Admin.ArticleImages;

public class AdminRemoveArticleImage : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "articles/{article:ArticleCode}/images/{orderIndex:int}", Handle)
            .WithName("AdminRemoveArticleImage").WithSummary("[admin] Remove an ArticleImage")
            .WithTags(nameof(ArticleImage))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<RemoveArticleImageResponse>, NotFound, ProblemHttpResult>> Handle(
        ArticleCode article,
        int orderIndex,
        IArticleRepository articleRepository,
        IArticleImageRepository articleImageRepository,
        IImageStorageService imageStorageService,
        ILogger<AdminRemoveArticleImage> logger,
        CancellationToken cancellationToken)
    {
        var articleEntity = await articleRepository.GetBy(article, cancellationToken);
        if (articleEntity is null) return TypedResults.NotFound();

        var entity = await articleImageRepository.GetBy(article, orderIndex, cancellationToken);

        if (entity is null) return TypedResults.NotFound();

        var isDeleted = await articleImageRepository.Delete(article, orderIndex, cancellationToken);
        if (!isDeleted)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Unable to delete article image",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "The image could not be removed."
            });

        try
        {
            await imageStorageService.DeleteAsync(entity.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Deleted article image '{ImageId}' for article '{ArticleCode}' but failed to delete its blob.",
                entity.Id,
                article);
        }

        return TypedResults.Ok(MapToResponse(entity, article));
    }

    public record RemoveArticleImageResponse
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public required string ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
        public required ArticleCode ArticleCode { get; init; }
        public string Message { get; init; } = "Article image removed successfully.";
    }

    public static RemoveArticleImageResponse MapToResponse(ArticleImage entity, ArticleCode article)
    {
        return new RemoveArticleImageResponse
        {
            Id = entity.Id,
            OrderIndex = entity.OrderIndex,
            ArticleCode = article,
            ImageUrl = entity.ImageUrl,
            ImageAlt = entity.ImageAlt
        };
    }
}