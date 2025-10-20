using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Catalog.API.Helpers;
using Catalog.API.Models.ProductDetailModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Iterfaces;
using ProductDetail = Catalog.API.Entities.ProductDetail;

namespace Catalog.API.Endpoints.ProductDetails;

public class CreateProductDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("ProductDetails", Handler)
            .WithName("CreateProductDetail").WithSummary("Create a new ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces<ProductDetailBaseResponseModel>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<ProductDetailBaseResponseModel>, BadRequest<ProblemDetails>>> Handler(
        ProductDetailBaseRequestModel requestModel,
        HttpContext httpContext,
        IProductDetailRepository productDetailRepository,
        IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

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

        var productDetailModel = mapper.Map<ProductDetail>(requestModel);
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
            mapper.Map<ProductDetailBaseResponseModel>(createdProductDetail));
    }
}