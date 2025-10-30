using Asp.Versioning;
using Catalog.Api.Infrastructure.Abstractions;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Features.Products.Update;

public class UpdateProductByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Products/{id:guid}", Handler)
            .WithName("UpdateProductById").WithSummary("Update a Product by id.")
            .WithTags("Products")
            .Produces<UpdateProductByIdResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<UpdateProductByIdResponse>, NotFound, BadRequest<ProblemDetails>>> Handler(
        Guid id,
        UpdateProductByIdRequest request,
        IProductRepository productRepository)
    {
        var entity = await productRepository.GetById(id);
        if (entity is null) return TypedResults.NotFound();

        entity.ApplyUpdate(request);

        var updated = await productRepository.UpdateAsync(entity);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(updated.ToResponse());
    }
}