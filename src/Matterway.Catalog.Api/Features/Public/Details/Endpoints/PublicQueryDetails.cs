using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Public.Details.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

namespace Matterway.Catalog.Api.Features.Public.Details.Endpoints;

public class PublicQueryDetails : IEndpoint
{
    private const string RouteName = nameof(PublicQueryDetails);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Public, "details", Handle)
            .WithName(RouteName).WithSummary("[public] Query available detail definitions")
            .WithTags(nameof(Detail))
            .Produces<PaginationResponse<PublicQueryDetailResponse>>()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Ok<PaginationResponse<PublicQueryDetailResponse>>> Handle(
        [AsParameters]
        PublicQueryDetailRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        IDetailRepository detailRepository,
        CancellationToken cancellationToken)
    {
        var total = await detailRepository.Count(request.TitleLike, cancellationToken);
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
                nameof(PublicQueryDetailRequest.TitleLike),
                request.TitleLike);

        var response = entities.Select(ToResponse).ToList();
        var paginationResponse = PaginationResponse<PublicQueryDetailResponse>.Create(
            response,
            total,
            request.Page,
            request.PageSize,
            location);
        return TypedResults.Ok(paginationResponse);
    }

    private static PublicQueryDetailResponse ToResponse(Detail entity)
    {
        return new PublicQueryDetailResponse
        {
            Slug = entity.Slug,
            Title = entity.Title,
            Unit = entity.Unit
        };
    }
}
