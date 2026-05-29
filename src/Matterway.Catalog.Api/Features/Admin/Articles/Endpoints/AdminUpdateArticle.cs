using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.Articles.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

namespace Matterway.Catalog.Api.Features.Admin.Articles.Endpoints;

public class AdminUpdateArticle : IEndpoint
{
    private const string RouteName = nameof(AdminUpdateArticle);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "articles/{code:ArticleCode}", Handle)
            .WithName(RouteName).WithSummary("[admin] Update an Article")
            .WithTags("Articles")
            .Produces<AdminUpdateArticleResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminUpdateArticleResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        ArticleCode code,
        AdminUpdateArticleRequest request,
        IArticleRepository articleRepository,
        CancellationToken cancellationToken)
    {
        var entity = await articleRepository.GetBy(code, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        ApplyToEntity(entity, request);

        var updated = await articleRepository.Update(entity, code, cancellationToken);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(ToResponse(updated));
    }

    private static void ApplyToEntity(Article entity, AdminUpdateArticleRequest request)
    {
        entity.UpdateDetails(request.Code, request.Title, request.Description);
        if (request.IsAvailable.HasValue)
            entity.SetAvailability(request.IsAvailable.Value);

        if (request.BasePrice is not null)
            entity.SetBasePrice(request.BasePrice.Value);
    }

    private static AdminUpdateArticleResponse ToResponse(Article entity)
    {
        return new AdminUpdateArticleResponse
        {
            Code = ArticleCode.Parse(entity.ArticleCode, null),
            Title = entity.Title,
            BasePrice = entity.BasePrice,
            Price = entity.GetFinalPrice(),
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsAvailable = entity.IsAvailable
        };
    }
}
