using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Catalog.API.Features.Products.Contracts;
using Catalog.API.Features.Products.Data;
using Catalog.API.Features.Products.Domain;
using Catalog.API.Features.Products.Shared;
using Catalog.API.Features.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Iterfaces;
using SharedProject.ModelTemplates;

namespace Catalog.API.Features.Products.Endpoints;

public class QueryProducts : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Products", Handler)
            .WithName("QueryProducts").WithSummary("Query Products.")
            .WithTags("Products")
            .Produces<PaginationResponse<ProductBaseResponse>>(StatusCodes.Status200OK)
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
            IProductRepository productRepository,
            IMapper mapper)
    {
        var total = await productRepository.GetTotalEntities(productFilter);
        var entities =
            await productRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value, productFilter);
        var baseUri = ResourceUrlHelper.CreateBaseUri(httpContext, "Products");

        var paginationResponse = new PaginationResponse<ProductBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<ProductBaseResponse>>(entities).ToList(), baseUri);

        return entities is IEnumerable<Product> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}