using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.SpecificationEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Admin.Specifications;

public class AdminDeleteSpecification : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("admin/specifications/{slug}", Handle)
            .WithName("AdminDeleteSpecification").WithSummary("Delete a Specification definition (admin).")
            .WithTags(nameof(Specification))
            .Produces<DeleteSpecificationResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
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
                Detail = "Specification is referenced by one or more articles.",
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