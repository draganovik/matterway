using Asp.Versioning;
using Catalog.Api.Features.ProductDetails.Contracts;
using Catalog.Api.Features.ProductDetails.Data;
using Catalog.Api.Features.ProductDetails.Domain;
using Catalog.Api.Features.ProductDetails.Mapping;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Features.ProductDetails.Endpoints;

public class UpdateProductDetailById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("ProductDetails/{id:guid}", Handler)
            .WithName("UpdateProductDetailById").WithSummary("Update a ProductDetail by id.")
            .WithTags(nameof(ProductDetail))
            .Produces<ProductDetailBaseResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductDetailBaseResponse>, NotFound, BadRequest<ProblemDetails>>>
        Handler(
            Guid id,
            ProductDetailBaseRequest request,
            IProductDetailRepository productDetailRepository)
    {
        var updatedProductDetail = await productDetailRepository.Update(id, request);
        return updatedProductDetail is not null
            ? TypedResults.Ok(updatedProductDetail.ToContract<ProductDetailBaseResponse>())
            : TypedResults.NotFound();
    }
}