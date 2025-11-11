using System.ComponentModel.DataAnnotations;
using System.Linq;
using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetailType;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
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
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
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
        public int Id { get; init; }
        public string? Title { get; init; }
        public string? Unit { get; init; }
    }

    public static QueryProductDetailTypeResponse MapToResponse(ProductDetailType entity)
    {
        return new QueryProductDetailTypeResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            Unit = entity.Unit
        };
    }
}