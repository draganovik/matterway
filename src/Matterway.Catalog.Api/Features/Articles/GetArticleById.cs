using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Articles;

public class GetArticleById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Articles/{id:guid}", Handle)
            .WithName("GetArticleById").WithSummary("Get an Article.")
            .WithTags("Articles")
            .Produces<GetArticleByIdResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<GetArticleByIdResponse>, NotFound>> Handle(
        Guid id,
        IArticleRepository articleRepository,
        CancellationToken cancellationToken)
    {
        var article = await articleRepository.GetBy(id, cancellationToken);

        if (article == null) return TypedResults.NotFound();

        return TypedResults.Ok(MapToResponse(article));
    }

    public record GetArticleByIdResponse
    {
        public Guid Id { get; init; }
        public string? Code { get; init; }
        public string? Title { get; init; }
        public decimal? BasePrice { get; init; }
        public decimal? Price { get; init; }
        public ArticleDiscountProperty? Discount { get; set; }
        public string? Description { get; init; }
        public ICollection<ArticleDetailProperty>? Details { get; init; } = [];
        public ICollection<ArticleSpecificationProperty>? Specifications { get; init; } = [];
        public ICollection<ArticleImageProperty>? Images { get; init; } = [];
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public bool IsAvailable { get; init; }
    }

    public record ArticleDiscountProperty
    {
        public decimal Percentage { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
    }

    public record ArticleDetailProperty
    {
        public string? DetailSlug { get; init; }
        public string? Title { get; init; }
        public string? Value { get; init; }
    }

    public record ArticleSpecificationProperty
    {
        public string? SpecificationSlug { get; init; }
        public string? Title { get; init; }
        public decimal Value { get; init; }
        public string? Unit { get; init; }
    }

    public record ArticleImageProperty
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
    }

    public static GetArticleByIdResponse MapToResponse(Article entity)
    {
        var basePrice = entity.BasePrice;
        var price = entity.GetFinalPrice();
        var discount = entity.GetLatestActiveDiscount();
        return new GetArticleByIdResponse
        {
            Id = entity.Id,
            Code = entity.ArticleCode,
            Title = entity.Title,
            BasePrice = basePrice,
            Price = price,
            Discount = discount is null
                ? null
                : new ArticleDiscountProperty
                {
                    Percentage = discount.Percentage,
                    ValidFrom = discount.ValidFrom,
                    ValidTo = discount.ValidTo
                },
            Description = entity.Description,
            Details = entity.ArticleDetails?
                .Select(MapDetailToResponse).ToList(),
            Specifications = entity.ArticleSpecifications?
                .Select(MapSpecificationToResponse).ToList(),
            Images = entity.ArticleImages?
                .OrderBy(pi => pi.OrderIndex)
                .Select(MapImageToResponse).ToList(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsAvailable = entity.IsAvailable
        };
    }

    public static ArticleDetailProperty MapDetailToResponse(ArticleDetail entity)
    {
        return new ArticleDetailProperty
        {
            DetailSlug = entity.DetailSlug,
            Title = entity.Detail?.Title,
            Value = entity.Value
        };
    }

    public static ArticleSpecificationProperty MapSpecificationToResponse(ArticleSpecification entity)
    {
        return new ArticleSpecificationProperty
        {
            SpecificationSlug = entity.SpecificationSlug,
            Title = entity.Specification?.Title,
            Value = entity.Value,
            Unit = entity.Specification?.Unit
        };
    }

    public static ArticleImageProperty MapImageToResponse(ArticleImage entity)
    {
        return new ArticleImageProperty
        {
            Id = entity.Id,
            OrderIndex = entity.OrderIndex,
            ImageUrl = entity.ImageUrl,
            ImageAlt = entity.ImageAlt
        };
    }
}