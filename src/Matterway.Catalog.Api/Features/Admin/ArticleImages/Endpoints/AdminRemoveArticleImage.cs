using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.ArticleImages.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;
using Matterway.Catalog.Api.Infrastructure.Storage;

namespace Matterway.Catalog.Api.Features.Admin.ArticleImages.Endpoints;

public class AdminRemoveArticleImage : IEndpoint
{
    private const string RouteName = nameof(AdminRemoveArticleImage);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "articles/{article:ArticleCode}/images/{orderIndex:int}", Handle)
            .WithName(RouteName).WithSummary("[admin] Remove an ArticleImage")
            .WithTags(nameof(ArticleImage))
            .Produces<AdminRemoveArticleImageResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminRemoveArticleImageResponse>, NotFound, ProblemHttpResult>> Handle(
        ArticleCode article,
        int orderIndex,
        IArticleImageRepository articleImageRepository,
        IImageStorageService imageStorageService,
        ILogger<AdminRemoveArticleImage> logger,
        CancellationToken cancellationToken)
    {
        var entity = await articleImageRepository.GetBy(article, orderIndex, cancellationToken);

        if (entity is null) return TypedResults.NotFound();

        var isDeleted = await articleImageRepository.Delete(article, orderIndex, cancellationToken);
        if (!isDeleted)
        {
            var existingImage = await articleImageRepository.GetBy(article, orderIndex, cancellationToken);
            if (existingImage is null) return TypedResults.NotFound();

            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Unable to delete article image",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "The image could not be removed."
            });
        }

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

        var refreshedImages = await articleImageRepository.GetByArticle(article, cancellationToken);
        return TypedResults.Ok(new AdminRemoveArticleImageResponse
        {
            Images = refreshedImages
                .Select(ToResponse)
                .ToList(),
            DeletedImageId = entity.Id
        });
    }

    private static AdminBaseArticleImageResponse ToResponse(ArticleImage entity)
    {
        return new AdminBaseArticleImageResponse
        {
            Id = entity.Id,
            OrderIndex = entity.OrderIndex,
            ImageUrl = entity.ImageUrl,
            ImageAlt = entity.ImageAlt
        };
    }
}