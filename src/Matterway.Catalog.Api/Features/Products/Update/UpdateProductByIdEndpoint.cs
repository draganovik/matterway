using Asp.Versioning;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Catalog.Api.Infrastructure.Persistence.Product;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Products.Update;

public class UpdateProductByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Products/{id:guid}", Handler)
            .WithName("UpdateProduct").WithSummary("Update a Product.")
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

        entity.MapUpdate(request);

        var updated = await productRepository.UpdateAsync(entity);

        if (updated is null) return TypedResults.NotFound();

        return TypedResults.Ok(updated.ToResponse());
    }
}