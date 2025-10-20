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

namespace Catalog.API.Features.ProductDetails.Endpoints;

public class CreateProductDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("ProductDetails", Handler)
            .WithName("CreateProductDetail").WithSummary("Create a new ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces<ProductDetailBaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<ProductDetailBaseResponse>, BadRequest<ProblemDetails>>> Handler(
        ProductDetailBaseRequest request,
        HttpContext httpContext,
        IProductDetailRepository productDetailRepository,
        IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(request);
        var isValid = Validator.TryValidateObject(request, context, results, true);

        if (!isValid)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred."
            };
            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var productDetailModel = mapper.Map<ProductDetail>(request);
        var createdProductDetail = await productDetailRepository.Create(productDetailModel);
        if (createdProductDetail is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var location = ResourceUrlHelper.BuildResourceLocation(httpContext,
            $"ProductDetails/{createdProductDetail.Id}");

        return TypedResults.Created(location,
            mapper.Map<ProductDetailBaseResponse>(createdProductDetail));
    }
}