using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Storage;

namespace Matterway.Catalog.Api.Endpoints.Admin.Articles;

public class AdminDeleteArticle : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "articles/{code:ArticleCode}", Handle)
            .WithName("AdminDeleteArticle").WithSummary("[admin] Delete an Article")
            .WithTags("Articles")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<DeleteArticleResponse>, NotFound, ProblemHttpResult>> Handle(
        ArticleCode code,
        IArticleRepository articleRepository,
        IImageStorageService imageStorageService,
        ILogger<AdminDeleteArticle> logger,
        CancellationToken cancellationToken)
    {
        var article = await articleRepository.GetBy(code, cancellationToken);

        if (article is null) return TypedResults.NotFound();

        var imageIds = article.ArticleImages?.Select(image => image.Id).ToArray() ?? [];

        var isDeleted = await articleRepository.Delete(code, cancellationToken);
        if (!isDeleted)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Unable to delete article",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "The article could not be deleted."
            });

        foreach (var imageId in imageIds)
            try
            {
                await imageStorageService.DeleteAsync(imageId, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "Deleted article '{ArticleCode}' but failed to delete image blob '{ImageId}'.",
                    code,
                    imageId);
            }

        var response = new DeleteArticleResponse
        {
            Code = code
        };
        return TypedResults.Ok(response);
    }

    public record DeleteArticleResponse
    {
        public required ArticleCode Code { get; init; }
        public string Message { get; init; } = "Article deleted successfully.";
    }
}