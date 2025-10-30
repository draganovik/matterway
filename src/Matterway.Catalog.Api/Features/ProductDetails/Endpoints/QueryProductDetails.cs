using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Features.ProductDetails.Mapping;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Features.ProductDetails.Contracts;
using Matterway.Catalog.Api.Infrastructure.Abstractions;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Http;
using Matterway.Common.Pagination;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductDetails.Endpoints;

public class QueryProductDetails : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ProductDetails", Handler)
            .WithName("QueryProductDetails").WithSummary("Query ProductDetails.")
            .WithTags(nameof(ProductDetail))
            .Produces<PaginationResponse<ProductDetailBaseResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async
        Task<Results<Ok<PaginationResponse<ProductDetailBaseResponse>>, NoContent, BadRequest<ProblemDetails>>>
        Handler([FromQuery] [Range(1, int.MaxValue)] int page,
            [FromQuery] [Range(1, int.MaxValue)]
            int pageSize,
            HttpContext httpContext,
            IProductDetailRepository productDetailRepository)
    {
        var total = await productDetailRepository.GetTotalEntities();
        var entities = await productDetailRepository.Query(page, pageSize);
        var baseUri = ApiResourceUriBuilder.BuildAbsoluteUri(httpContext, "ProductDetails");

        var paginationResponse = new PaginationResponse<ProductDetailBaseResponse>(total, page, pageSize,
            entities.Select(entity => entity.ToContract<ProductDetailBaseResponse>()
            ).ToList()
            , baseUri);

        return entities is IEnumerable<ProductDetail> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}