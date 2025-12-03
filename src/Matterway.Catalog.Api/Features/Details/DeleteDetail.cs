using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Details;

public class DeleteDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Details/{slug}", Handle)
            .WithName("DeleteDetail").WithSummary("Delete a Detail definition.")
            .WithTags(nameof(Detail))
            .Produces<DeleteDetailResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<DeleteDetailResponse>, NotFound, BadRequest<ProblemDetails>>> Handle(
        string slug,
        IDetailRepository repository,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetBySlugAsync(slug, cancellationToken);
        if (entity is null) return TypedResults.NotFound();

        var deleted = await repository.DeleteAsync(slug, cancellationToken);
        if (!deleted)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot delete detail",
                Detail = "Detail is referenced by one or more products.",
                Status = StatusCodes.Status400BadRequest
            });

        return TypedResults.Ok(new DeleteDetailResponse
        {
            Slug = slug,
            Message = "Detail removed successfully."
        });
    }

    public record DeleteDetailResponse
    {
        public string Slug { get; init; } = default!;
        public string Message { get; init; } = "Detail removed successfully.";
    }
}