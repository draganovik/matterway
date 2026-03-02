using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

namespace Matterway.Catalog.Api.Features.Admin.Articles;

public class AdminCreateArticle : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "articles", Handle)
            .WithName("AdminCreateArticle").WithSummary("[admin] Create a new Article")
            .WithTags("Articles")
            .Produces<CreateArticleResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<CreateArticleResponse>, BadRequest<ProblemDetails>>> Handle(
        CreateArticleRequest request,
        IArticleRepository articleRepository,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        CancellationToken cancellationToken)
    {
        var article = MapToEntity(request);

        var created = await articleRepository.Create(article, cancellationToken);

        if (created is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Article could not be created.",
                Status = StatusCodes.Status400BadRequest
            });

        var location = linkGenerator.GetUriByName(
            httpContext,
            "PublicGetArticleById",
            new
            {
                id = created.Id
            });

        return TypedResults.Created(location, MapToResponse(created));
    }

    public record CreateArticleRequest
    {
        [Required]
        [RegularExpression(@"^[A-Z0-9]{5,10}$",
            ErrorMessage = "Article code must be 5-10 characters and only contain uppercase letters and numbers.")]
        public required string ArticleCode { get; init; }

        [Required]
        public required string Title { get; init; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Base price must be greater than zero.")]
        public required decimal BasePrice { get; init; }

        [Required]
        public required string Description { get; init; }

        public bool IsAvailable { get; init; }
    }

    public record CreateArticleResponse
    {
        public Guid Id { get; init; }
        public string? ArticleCode { get; init; }
        public string? Title { get; init; }
        public decimal? BasePrice { get; init; }
        public decimal? Price { get; init; }
        public string? Description { get; init; }
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public bool IsAvailable { get; init; }
    }

    public static Article MapToEntity(CreateArticleRequest request)
    {
        var article = new Article
        {
            ArticleCode = request.ArticleCode,
            Title = request.Title,
            Description = request.Description
        };
        article.SetBasePrice(request.BasePrice);
        article.SetAvailability(request.IsAvailable);
        return article;
    }

    public static CreateArticleResponse MapToResponse(Article entity)
    {
        return new CreateArticleResponse
        {
            Id = entity.Id,
            ArticleCode = entity.ArticleCode,
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