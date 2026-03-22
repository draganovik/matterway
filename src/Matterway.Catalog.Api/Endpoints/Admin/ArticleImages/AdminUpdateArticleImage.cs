using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;

namespace Matterway.Catalog.Api.Endpoints.Admin.ArticleImages;

public class AdminUpdateArticleImage : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "articles/{article:ArticleCode}/images/{orderIndex:int}", Handle)
            .WithName("AdminUpdateArticleImage").WithSummary("[admin] Update an ArticleImage")
            .WithTags(nameof(ArticleImage))
            .Produces<UpdateArticleImageResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<UpdateArticleImageResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        ArticleCode article,
        int orderIndex,
        UpdateArticleImageRequest request,
        IArticleRepository articleRepository,
        IArticleImageRepository articleImageRepository,
        CancellationToken cancellationToken)
    {
        var articleEntity = await articleRepository.GetBy(article, cancellationToken);
        if (articleEntity is null) return TypedResults.NotFound();

        var entity = await articleImageRepository.GetBy(article, orderIndex, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        entity.ImageAlt = request.ImageAlt ?? entity.ImageAlt;

        var targetOrderIndex = request.OrderIndex ?? entity.OrderIndex;

        if (targetOrderIndex < 0) targetOrderIndex = 0;

        var updated = await articleImageRepository.Update(entity, targetOrderIndex, cancellationToken);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(new UpdateArticleImageResponse
        {
            Id = updated.Id,
            OrderIndex = updated.OrderIndex,
            ArticleCode = article,
            ArticleName = articleEntity.Title,
            ImageUrl = updated.ImageUrl,
            ImageAlt = updated.ImageAlt
        });
    }

    public record UpdateArticleImageRequest
    {
        public string? ImageAlt { get; init; }

        [Range(0, int.MaxValue)]
        public int? OrderIndex { get; init; }
    }

    public record UpdateArticleImageResponse
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public required ArticleCode ArticleCode { get; init; }
        public string? ArticleName { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
    }
}