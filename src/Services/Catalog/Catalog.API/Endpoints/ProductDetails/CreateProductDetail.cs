using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductDetailModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models;
using ProductDetail = Catalog.API.Entities.ProductDetail;

namespace Catalog.API.Endpoints.ProductDetails;

public class CreateProductDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/ProductDetails", Handler)
            .WithName("CreateProductDetail").WithSummary("Create a new ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces<ProductDetailBaseResponseModel>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)));
    }

    private static async Task<Results<Created<ProductDetailBaseResponseModel>, BadRequest<ProblemDetails>>> Handler(
        ProductDetailBaseRequestModel requestModel,
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

        return TypedResults.Created($"/api/ProductDetails/{createdProductDetail.Id}",
            mapper.Map<ProductDetailBaseResponseModel>(createdProductDetail));
    }
}
