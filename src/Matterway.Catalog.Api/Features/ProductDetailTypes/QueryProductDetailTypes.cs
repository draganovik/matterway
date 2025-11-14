using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetailType;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.ProductDetailTypes;

public class QueryProductDetailTypes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ProductDetailTypes", Handle)
            .WithName("QueryProductDetailTypes").WithSummary("Query available ProductDetail types.")
            .WithTags(nameof(ProductDetailType))
            .Produces<ICollection<QueryProductDetailTypeResponse>>()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(RequestClaimsRole.Admin),
                nameof(RequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Ok<ICollection<QueryProductDetailTypeResponse>>> Handle(
        [AsParameters]
        QueryProductDetailTypeRequest request,
        IProductDetailTypeRepository productDetailTypeRepository,
        CancellationToken cancellationToken)
    {
        var entities =
            await productDetailTypeRepository.QueryAsync(request.TitleLike, request.Limit, cancellationToken);
        var response = entities.Select(MapToResponse).ToList();
        return TypedResults.Ok<ICollection<QueryProductDetailTypeResponse>>(response);
    }

    public record QueryProductDetailTypeRequest
    {
        [Range(1, 50)]
        public int Limit { get; init; } = 10;

        public string? TitleLike { get; init; }
    }

    public record QueryProductDetailTypeResponse
    {
        public string? Slug { get; init; }
        public string? Title { get; init; }
        public string? Unit { get; init; }
    }

    public static QueryProductDetailTypeResponse MapToResponse(ProductDetailType entity)
    {
        return new QueryProductDetailTypeResponse
        {
            Slug = entity.Slug,
            Title = entity.Title,
            Unit = entity.Unit
        };
    }
}