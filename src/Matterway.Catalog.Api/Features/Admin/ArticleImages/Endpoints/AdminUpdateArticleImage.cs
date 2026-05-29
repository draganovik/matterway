using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.ArticleImages.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;

namespace Matterway.Catalog.Api.Features.Admin.ArticleImages.Endpoints;

public class AdminUpdateArticleImage : IEndpoint
{
    private const string RouteName = nameof(AdminUpdateArticleImage);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "articles/{article:ArticleCode}/images/{orderIndex:int}", Handle)
            .WithName(RouteName).WithSummary("[admin] Update an ArticleImage")
            .WithTags(nameof(ArticleImage))
            .Produces<AdminUpdateArticleImageResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminUpdateArticleImageResponse>, NotFound, BadRequest<ProblemDetails>>>
        Handle(
            ArticleCode article,
            int orderIndex,
            AdminUpdateArticleImageRequest request,
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

        return TypedResults.Ok(new AdminUpdateArticleImageResponse
        {
            Images = refreshedImages
                .Select(ToResponse)
                .ToList(),
            UpdatedImageId = entity.Id
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