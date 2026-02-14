using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.ServiceDefaults.Api;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Admin.Details;

public class AdminQueryDetails : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "details", Handle)
            .WithName("AdminQueryDetails").WithSummary("[admin] Query available detail definitions")
            .WithTags(nameof(Detail))
            .Produces<ICollection<QueryDetailResponse>>()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsObserver(context.User)))
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
        public string? Unit { get; init; }
    }

    public static QueryDetailResponse MapToResponse(Detail entity)
    {
        return new QueryDetailResponse
        {
            Slug = entity.Slug,
            Title = entity.Title,
            Unit = entity.Unit
        };
    }
}