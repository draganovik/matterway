using Asp.Versioning;
using Catalog.Api.Domain;
using Catalog.Api.Features.Products.Contracts;
using Catalog.Api.Features.Products.Data;
using Catalog.Api.Features.Products.Mapping;
using Catalog.Api.Features.Products.Shared;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Http;
using Common.Infrastructure.Pagination;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Features.Products.Endpoints;

public class QueryProducts : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Products", Handler)
            .WithName("QueryProducts").WithSummary("Query Products.")
            .WithTags("Products")
            .Produces<PaginationResponse<ProductBaseResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async
        Task<Results<Ok<PaginationResponse<ProductBaseResponse>>, NoContent, BadRequest<ProblemDetails>,
            ValidationProblem>>
        Handler([AsParameters] PagingQueryParams pagingQuery,
            [AsParameters]
            ProductFilter productFilter,
            HttpContext httpContext,
            IProductRepository productRepository)
    {
        var total = await productRepository.GetTotalEntities(productFilter);
        var entities =
            await productRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value, productFilter);
        var baseUri = ApiResourceUriBuilder.BuildAbsoluteUri(httpContext, "Products");

        var paginationResponse = new PaginationResponse<ProductBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            entities.Select(entity => entity.ToContract<ProductBaseResponse>()
            ).ToList()
            , baseUri);

        return entities is IEnumerable<Product> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}