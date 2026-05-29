using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Features.Admin.Details.Contracts;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

namespace Matterway.Catalog.Api.Features.Admin.Details.Endpoints;

public class AdminPutDetail : IEndpoint
{
    private const string RouteName = nameof(AdminPutDetail);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPut(EndpointKind.Admin, "details/{slug}", Handle)
            .WithName(RouteName).WithSummary("[admin] Create or replace a Detail definition")
            .WithTags(nameof(Detail))
            .Produces<AdminPutDetailResponse>(StatusCodes.Status200OK)
            .Produces<AdminPutDetailResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminPutDetailResponse>, Created<AdminPutDetailResponse>,
            BadRequest<ProblemDetails>>>
        Handle(
            string slug,
            AdminPutDetailRequest request,
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

        var response = ToResponse(saved);
        if (existing is null)
        {
            var location = $"{httpContext.Request.Path}";
            return TypedResults.Created(location, response);
        }

        return TypedResults.Ok(response);
    }

    private static AdminPutDetailResponse ToResponse(Detail entity)
    {
        return new AdminPutDetailResponse
        {
            Slug = entity.Slug,
            Title = entity.Title,
            Unit = entity.Unit
        };
    }
}