using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

namespace Matterway.Catalog.Api.Endpoints.Public.Details;

public class PublicQueryDetails : IEndpoint
{
    private const string RouteName = nameof(PublicQueryDetails);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Public, "details", Handle)
            .WithName(RouteName).WithSummary("[public] Query available detail definitions")
            .WithTags(nameof(Detail))
            .Produces<PaginationResponse<QueryDetailResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<PaginationResponse<QueryDetailResponse>>, NoContent>> Handle(
        [AsParameters]
        QueryDetailRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        IDetailRepository detailRepository,
        CancellationToken cancellationToken)
    {
        var total = await detailRepository.Count(request.TitleLike, cancellationToken);
        if (total == 0)
            return TypedResults.NoContent();

        var entities = await detailRepository.Query(
            request.Page,
            request.PageSize,
            request.TitleLike,
            cancellationToken);

        var location = linkGenerator.GetUriByName(
            httpContext,
            RouteName,
            null);

        if (!string.IsNullOrWhiteSpace(location) && !string.IsNullOrWhiteSpace(request.TitleLike))
            location = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(
                location,
                nameof(QueryDetailRequest.TitleLike),
                request.TitleLike);

        var response = entities.Select(MapToResponse).ToList();
        var paginationResponse = PaginationResponse<QueryDetailResponse>.Create(
            response,
            total,
            request.Page,
            request.PageSize,
            location);
        return TypedResults.Ok(paginationResponse);
    }

    public sealed record QueryDetailRequest : PaginationRequestParameters
    {
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