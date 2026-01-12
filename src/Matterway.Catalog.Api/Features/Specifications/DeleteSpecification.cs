using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Providers.Persistence.SpecificationEntity;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Specifications;

public class DeleteSpecification : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Specifications/{slug}", Handle)
            .WithName("DeleteSpecification").WithSummary("Delete a Specification definition.")
            .WithTags(nameof(Specification))
            .Produces<DeleteSpecificationResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestRole.Admin),
                nameof(ERequestRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteSpecificationResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        string slug,
        ISpecificationRepository repository,
        CancellationToken cancellationToken)
    {
        var normalizedSlug = slug.Trim().ToLower();
        var entity = await repository.GetBy(normalizedSlug, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        var deleted = await repository.Delete(normalizedSlug, cancellationToken);
        if (!deleted)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot delete specification",
                Detail = "Specification is referenced by one or more products.",
                Status = StatusCodes.Status400BadRequest
            });

        return TypedResults.Ok(new DeleteSpecificationResponse
        {
            Slug = normalizedSlug,
            Message = "Specification removed successfully."
        });
    }

    public record DeleteSpecificationResponse
    {
        public string Slug { get; init; } = default!;
        public string Message { get; init; } = "Specification removed successfully.";
    }
}