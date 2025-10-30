using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Features.ProductImages.Mapping;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Features.ProductImages.Contracts;
using Matterway.Catalog.Api.Infrastructure.Abstractions;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Http;
using Matterway.Common.Pagination;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductImages.Endpoints;

public class QueryProductImages : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ProductImages", Handler)
            .WithName("QueryProductImages").WithSummary("Query ProductImages.")
            .WithTags(nameof(ProductImage))
            .Produces<PaginationResponse<ProductImageBaseResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async
        Task<Results<Ok<PaginationResponse<ProductImageBaseResponse>>, NoContent, BadRequest<ProblemDetails>>>
        Handler([FromQuery] [Range(1, int.MaxValue)] int page,
            [FromQuery] [Range(1, int.MaxValue)]
            int pageSize,
            HttpContext httpContext,
            IProductImageRepository productImageRepository)
    {
        var total = await productImageRepository.GetTotalEntities();
        var entities = await productImageRepository.Query(page, pageSize);
        var baseUri = ApiResourceUriBuilder.BuildAbsoluteUri(httpContext, "ProductImages");

        var paginationResponse = new PaginationResponse<ProductImageBaseResponse>(total, page, pageSize,
            entities.Select(entity => entity.ToContract<ProductImageBaseResponse>()
            ).ToList(), baseUri);

        return entities is IEnumerable<ProductImage> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}