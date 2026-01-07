using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Providers.Persistence.DetailEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Details;

public class QueryDetails : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Details", Handle)
            .WithName("QueryDetails").WithSummary("Query available detail definitions.")
            .WithTags(nameof(Detail))
            .Produces<ICollection<QueryDetailResponse>>()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestRole.Admin),
                nameof(ERequestRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Ok<ICollection<QueryDetailResponse>>> Handle(
        [AsParameters]
        QueryDetailRequest request,
        IDetailRepository detailRepository,
        CancellationToken cancellationToken)
    {
        var entities = await detailRepository.Query(request.TitleLike, request.Limit, cancellationToken);
        var response = entities.Select(MapToResponse).ToList();
        return TypedResults.Ok<ICollection<QueryDetailResponse>>(response);
    }

    public record QueryDetailRequest
    {
        [Range(1, 50)]
        public int Limit { get; init; } = 10;

        public string? TitleLike { get; init; }
    }

    public record QueryDetailResponse
    {
        public string? Slug { get; init; }
        public string? Title { get; init; }
    }

    public static QueryDetailResponse MapToResponse(Detail entity)
    {
        return new QueryDetailResponse
        {
            Slug = entity.Slug,
            Title = entity.Title
        };
    }
}