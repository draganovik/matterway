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
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<DeleteArticleResponse>, NotFound>> Handle(
        ArticleCode code,
        IArticleRepository articleRepository,
        IImageStorageService imageStorageService,
        CancellationToken cancellationToken)
    {
        var article = await articleRepository.GetBy(code, cancellationToken);

        if (article is null) return TypedResults.NotFound();

        var imageIds = article.ArticleImages?
            .Select(image => image.Id)
            .ToList();

        var isDeleted = await articleRepository.Delete(code, cancellationToken);
        if (!isDeleted) return TypedResults.NotFound();

        if (imageIds is not null)
            foreach (var imageId in imageIds)
                await imageStorageService.DeleteAsync(imageId, cancellationToken);

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