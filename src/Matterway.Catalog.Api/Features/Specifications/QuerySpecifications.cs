using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.EntitySpecification;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Specifications;

public class QuerySpecifications : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Specifications", Handle)
            .WithName("QuerySpecifications").WithSummary("Query available specifications.")
            .WithTags(nameof(Specification))
            .Produces<ICollection<QuerySpecificationResponse>>()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Ok<ICollection<QuerySpecificationResponse>>> Handle(
        [AsParameters]
        QuerySpecificationRequest request,
        ISpecificationRepository specificationRepository,
        CancellationToken cancellationToken)
    {
        var entities =
            await specificationRepository.QueryAsync(request.TitleLike, request.Limit, cancellationToken);
        var response = entities.Select(MapToResponse).ToList();
        return TypedResults.Ok<ICollection<QuerySpecificationResponse>>(response);
    }

    public record QuerySpecificationRequest
    {
        [Range(1, 50)]
        public int Limit { get; init; } = 10;

        public string? TitleLike { get; init; }
    }

    public record QuerySpecificationResponse
    {
        public string? Slug { get; init; }
        public string? Title { get; init; }
        public string? Unit { get; init; }
    }

    public static QuerySpecificationResponse MapToResponse(Specification entity)
    {
        return new QuerySpecificationResponse
        {
            Slug = entity.Slug,
            Title = entity.Title,
            Unit = entity.Unit
        };
    }
}