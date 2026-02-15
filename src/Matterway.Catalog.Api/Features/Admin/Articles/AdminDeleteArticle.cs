using Asp.Versioning;
using Matterway.ServiceDefaults.Api;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Storage;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Admin.Articles;

public class AdminDeleteArticle : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapDelete(EndpointKind.Admin, "articles/{id:guid}", Handle)
            .WithName("AdminDeleteArticle").WithSummary("[admin] Delete an Article")
            .WithTags("Articles")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteArticleResponse>, NotFound>> Handle(
        Guid id,
        IArticleRepository articleRepository,
        IImageStorageService imageStorageService,
        CancellationToken cancellationToken)
    {
        var article = await articleRepository.GetBy(id, cancellationToken);

        if (article is null) return TypedResults.NotFound();

        var imageIds = article.ArticleImages?
            .Select(image => new { image.ArticleId, image.Id })
            .ToList();

        var isDeleted = await articleRepository.Delete(id, cancellationToken);
        if (!isDeleted) return TypedResults.NotFound();

        if (imageIds is not null)
            foreach (var image in imageIds)
                await imageStorageService.DeleteAsync(image.ArticleId, image.Id, cancellationToken);

        var response = new DeleteArticleResponse
        {
            Id = id
        };
        return TypedResults.Ok(response);
    }

    public record DeleteArticleResponse
    {
        public Guid Id { get; init; }
        public string Message { get; init; } = "Article deleted successfully.";
    }
}