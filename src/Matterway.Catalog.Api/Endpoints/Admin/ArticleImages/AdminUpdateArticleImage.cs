using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Endpoints.Public.Articles;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;

namespace Matterway.Catalog.Api.Endpoints.Admin.ArticleImages;

public class AdminUpdateArticleImage : IEndpoint
{
    private const string RouteName = nameof(AdminUpdateArticleImage);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "articles/{article:ArticleCode}/images/{orderIndex:int}", Handle)
            .WithName(RouteName).WithSummary("[admin] Update an ArticleImage")
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
        IArticleImageRepository articleImageRepository,
        CancellationToken cancellationToken)
    {
        var entity = await articleImageRepository.GetBy(article, orderIndex, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        entity.ImageAlt = request.ImageAlt ?? entity.ImageAlt;

        var targetOrderIndex = request.OrderIndex ?? entity.OrderIndex;

        if (targetOrderIndex < 0) targetOrderIndex = 0;

        if (await articleImageRepository.Update(entity, targetOrderIndex, cancellationToken) is null)
            return TypedResults.NotFound();

        var refreshedImages = await articleImageRepository.GetByArticle(article, cancellationToken);

        return TypedResults.Ok(new UpdateArticleImageResponse
        {
            Images = refreshedImages
                .Select(PublicGetArticleByCode.MapImageToResponse)
                .ToList(),
            UpdatedImageId = entity.Id
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
        public ICollection<PublicGetArticleByCode.ArticleImageProperty>? Images { get; init; } = [];
        public required Guid UpdatedImageId { get; init; }
    }
}