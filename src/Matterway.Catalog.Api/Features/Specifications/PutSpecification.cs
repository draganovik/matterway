using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.SpecificationEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Specifications;

public class PutSpecification : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("Specifications/{slug}", Handle)
            .WithName("PutSpecification").WithSummary("Create or replace a Specification definition.")
            .WithTags(nameof(Specification))
            .Produces<PutSpecificationResponse>(StatusCodes.Status200OK)
            .Produces<PutSpecificationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PutSpecificationResponse>, Created<PutSpecificationResponse>,
        BadRequest<ProblemDetails>>> Handle(
        string slug,
        PutSpecificationRequest request,
        ISpecificationRepository repository,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var normalizedSlug = slug.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(normalizedSlug))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Slug is required",
                Status = StatusCodes.Status400BadRequest
            });
        var existing = await repository.GetBy(normalizedSlug, cancellationToken);

        var entity = new Specification
        {
            Slug = normalizedSlug,
            Title = request.Title.Trim(),
            Unit = request.Unit?.Trim()
        };

        var saved = await repository.Upsert(entity, cancellationToken);
        if (saved is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot create or save specification",
                Detail = "Slug is already used by a detail or update failed.",
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

    public record PutSpecificationRequest
    {
        [Required]
        [StringLength(120, MinimumLength = 1)]
        public required string Title { get; init; }

        [StringLength(20)]
        public string? Unit { get; init; }
    }

    public record PutSpecificationResponse
    {
        public string? Slug { get; init; }
        public string? Title { get; init; }
        public string? Unit { get; init; }
    }

    public static PutSpecificationResponse MapToResponse(Specification entity)
    {
        return new PutSpecificationResponse
        {
            Slug = entity.Slug,
            Title = entity.Title,
            Unit = entity.Unit
        };
    }
}