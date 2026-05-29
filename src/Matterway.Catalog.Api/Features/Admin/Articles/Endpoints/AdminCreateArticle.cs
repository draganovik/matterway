using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.Articles.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

namespace Matterway.Catalog.Api.Features.Admin.Articles.Endpoints;

public class AdminCreateArticle : IEndpoint
{
    private const string RouteName = nameof(AdminCreateArticle);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "articles", Handle)
            .WithName(RouteName).WithSummary("[admin] Create a new Article")
            .WithTags("Articles")
            .Produces<AdminCreateArticleResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<AdminCreateArticleResponse>, BadRequest<ProblemDetails>>> Handle(
        AdminCreateArticleRequest request,
        IArticleRepository articleRepository,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        CancellationToken cancellationToken)
    {
        var article = ToEntity(request);

        var created = await articleRepository.Create(article, cancellationToken);

        if (created is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Article could not be created.",
                Status = StatusCodes.Status400BadRequest
            });

        var location = linkGenerator.GetUriByName(
            httpContext,
            "PublicGetArticleByCode",
            new
            {
                code = created.ArticleCode
            });

        return TypedResults.Created(location, ToResponse(created));
    }

    private static Article ToEntity(AdminCreateArticleRequest request)
    {
        var article = new Article
        {
            ArticleCode = request.Code.ToString(),
            Title = request.Title,
            Description = request.Description
        };
        article.SetBasePrice(request.BasePrice);
        article.SetAvailability(request.IsAvailable);
        return article;
    }

    private static AdminCreateArticleResponse ToResponse(Article entity)
    {
        return new AdminCreateArticleResponse
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
