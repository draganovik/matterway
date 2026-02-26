using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

namespace Matterway.Catalog.Api.Features.Admin.Details;

public class AdminPutDetail : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPut(EndpointKind.Admin, "details/{slug}", Handle)
            .WithName("AdminPutDetail").WithSummary("[admin] Create or replace a Detail definition")
            .WithTags(nameof(Detail))
            .Produces<PutDetailResponse>(StatusCodes.Status200OK)
            .Produces<PutDetailResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
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
        var normalizedSlug = slug.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(normalizedSlug))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Slug is required",
                Status = StatusCodes.Status400BadRequest
            });
        var existing = await repository.GetBy(normalizedSlug, cancellationToken);

        var entity = new Detail
        {
            Slug = normalizedSlug,
            Title = request.Title.Trim(),
            Unit = string.IsNullOrWhiteSpace(request.Unit) ? null : request.Unit.Trim()
        };

        var saved = await repository.Upsert(entity, cancellationToken);
        if (saved is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot create or save detail",
                Detail = "Detail update failed or unit conflicts with existing article details.",
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

        [StringLength(40)]
        public string? Unit { get; init; }
    }

    public record PutDetailResponse
    {
        public string? Slug { get; init; }
        public string? Title { get; init; }
        public string? Unit { get; init; }
    }

    public static PutDetailResponse MapToResponse(Detail entity)
    {
        return new PutDetailResponse
        {
            Slug = entity.Slug,
            Title = entity.Title,
            Unit = entity.Unit
        };
    }
}