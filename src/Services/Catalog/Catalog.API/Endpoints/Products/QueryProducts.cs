using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Catalog.API.Filters;
using Catalog.API.Helpers;
using Catalog.API.Models.ProductModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Iterfaces;
using SharedProject.ModelTemplates;
using Product = Catalog.API.Entities.Product;

namespace Catalog.API.Endpoints.Products;

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
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async
        Task<Results<Ok<PaginationResponse<ProductBaseResponse>>, NoContent, BadRequest<ProblemDetails>>>
        Handler([FromQuery] [Range(1, int.MaxValue)] int page,
            [FromQuery] [Range(1, int.MaxValue)]
            int pageSize,
            [AsParameters]
            ProductFilter productFilter,
            HttpContext httpContext,
            IProductRepository productRepository,
            IMapper mapper)
    {
        if (page < 1 || pageSize < 1)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid page or pageSize.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Page and pageSize must be greater than zero."
            };
            var results = new List<ValidationResult>();
            if (page < 1) results.Add(new ValidationResult("Page must be greater than zero.", new[] { nameof(page) }));
            if (pageSize < 1)
                results.Add(new ValidationResult("PageSize must be greater than zero.", new[] { nameof(pageSize) }));

            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var total = await productRepository.GetTotalEntities(productFilter);
        var entities = await productRepository.Query(page, pageSize, productFilter);
        var baseUri = ResourceUrlHelper.CreateBaseUri(httpContext, "Products");

        var paginationResponse = new PaginationResponse<ProductBaseResponse>(total, page, pageSize,
            mapper.Map<IEnumerable<ProductBaseResponse>>(entities).ToList(), baseUri);

        return entities is IEnumerable<Product> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}