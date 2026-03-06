using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

namespace Matterway.Catalog.Api.Endpoints.Admin.Articles;

public class AdminUpdateArticle : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPatch(EndpointKind.Admin, "articles/{id:guid}", Handle)
            .WithName("AdminUpdateArticle").WithSummary("[admin] Update an Article")
            .WithTags("Articles")
            .Produces<UpdateArticleResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<UpdateArticleResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        Guid id,
        UpdateArticleRequest request,
        IArticleRepository articleRepository,
        CancellationToken cancellationToken)
    {
        var entity = await articleRepository.GetBy(id, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        MapUpdate(entity, request);

        var updated = await articleRepository.Update(entity, cancellationToken);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(updated));
    }

    public record UpdateArticleRequest
    {
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Article code must be between 5 and 10 characters.")]
        [RegularExpression(@"^[A-Z0-9]{5,10}$",
            ErrorMessage = "Article code must contain only uppercase letters and numbers.")]
        public string? ArticleCode { get; init; }

        [MinLength(1, ErrorMessage = "Title cannot be empty if provided.")]
        public string? Title { get; init; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Base price must be greater than zero.")]
        public decimal? BasePrice { get; init; }

        [MinLength(1, ErrorMessage = "Description cannot be empty if provided.")]
        public string? Description { get; init; }

        public bool IsAvailable { get; init; }
    }

    public record UpdateArticleResponse
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

    public static void MapUpdate(Article entity, UpdateArticleRequest request)
    {
        entity.UpdateDetails(request.ArticleCode, request.Title, request.Description);
        entity.SetAvailability(request.IsAvailable);

        if (request.BasePrice is not null)
            entity.SetBasePrice(request.BasePrice.Value);
    }

    public static UpdateArticleResponse MapToResponse(Article entity)
    {
        return new UpdateArticleResponse
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