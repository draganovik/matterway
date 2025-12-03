using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Details;

public class PutDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("Details/{slug}", Handle)
            .WithName("PutDetail").WithSummary("Create or replace a Detail definition.")
            .WithTags(nameof(Detail))
            .Produces<PutDetailResponse>(StatusCodes.Status200OK)
            .Produces<PutDetailResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PutDetailResponse>, Created<PutDetailResponse>, BadRequest<ProblemDetails>>>
        Handle(
            string slug,
            PutDetailRequest request,
            IDetailRepository repository,
            HttpContext httpContext,
            CancellationToken cancellationToken)
    {
        var normalizedSlug = slug.Trim();
        if (string.IsNullOrWhiteSpace(normalizedSlug))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Slug is required",
                Status = StatusCodes.Status400BadRequest
            });
        var existing = await repository.GetBySlugAsync(normalizedSlug, cancellationToken);

        var entity = new Detail
        {
            Slug = normalizedSlug,
            Title = request.Title.Trim()
        };

        var saved = await repository.UpsertAsync(entity, cancellationToken);
        if (saved is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot save detail",
                Status = StatusCodes.Status400BadRequest
            });

        var response = MapToResponse(saved);
        if (existing is null)
        {
            var location = $"{httpContext.Request.Path}";
            return TypedResults.Created(location, response);
        }

        return TypedResults.Ok(response);
    }

    public record PutDetailRequest
    {
        [Required]
        [StringLength(120, MinimumLength = 1)]
        public required string Title { get; init; }
    }

    public record PutDetailResponse
    {
        public string? Slug { get; init; }
        public string? Title { get; init; }
    }

    public static PutDetailResponse MapToResponse(Detail entity)
    {
        return new PutDetailResponse
        {
            Slug = entity.Slug,
            Title = entity.Title
        };
    }
}