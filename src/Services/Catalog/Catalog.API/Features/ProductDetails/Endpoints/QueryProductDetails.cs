using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Catalog.API.Features.ProductDetails.Contracts;
using Catalog.API.Features.ProductDetails.Data;
using Catalog.API.Features.ProductDetails.Domain;
using Catalog.API.Features.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Iterfaces;
using SharedProject.ModelTemplates;

namespace Catalog.API.Features.ProductDetails.Endpoints;

public class QueryProductDetails : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ProductDetails", Handler)
            .WithName("QueryProductDetails").WithSummary("Query ProductDetails.")
            .WithTags(nameof(ProductDetail))
            .Produces<PaginationResponse<ProductDetailBaseResponse>>(StatusCodes.Status200OK)
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
            IProductDetailRepository productDetailRepository,
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
            if (page < 1) results.Add(new ValidationResult("Page must be greater than zero.", [nameof(page)]));
            if (pageSize < 1)
                results.Add(new ValidationResult("PageSize must be greater than zero.", [nameof(pageSize)]));

            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var total = await productDetailRepository.GetTotalEntities();
        var entities = await productDetailRepository.Query(page, pageSize);
        var baseUri = ResourceUrlHelper.CreateBaseUri(httpContext, "ProductDetails");

        var paginationResponse = new PaginationResponse<ProductDetailBaseResponse>(total, page, pageSize,
            mapper.Map<IEnumerable<ProductDetailBaseResponse>>(entities).ToList(), baseUri);

        return entities is IEnumerable<ProductDetail> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}