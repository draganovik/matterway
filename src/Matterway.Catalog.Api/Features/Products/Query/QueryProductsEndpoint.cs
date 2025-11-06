using Asp.Versioning;
using Matterway.Common.Abstractions;
using Matterway.Common.Pagination;
using Matterway.Catalog.Api.Infrastructure.Persistence.Product;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Features.Products.Query;

public class QueryProductsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Products", Handler)
            .WithName("QueryProducts").WithSummary("Query Products.")
            .WithTags("Products")
            .Produces<PaginationResponse<QueryProductResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<QueryProductResponse>>, NoContent>>
        Handler([AsParameters] PagingQueryParams pagingQuery, [AsParameters] QueryProductFilter queryProductFilter,
            HttpContext httpContext, IProductRepository productRepository)
    {
        var total = await productRepository.GetTotalEntities(queryProductFilter);
        if (total == 0)
            return TypedResults.NoContent();

        var entities = await productRepository.Query(
            pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            queryProductFilter);

        var location = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}/Products");

        var results = entities.Select(entity => entity.ToResponse()).ToList();

        var paginationResponse = new PaginationResponse<QueryProductResponse>(
            total,
            pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            results,
            location);

        return TypedResults.Ok(paginationResponse);
    }
}