using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductImageModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models;
using ProductImage = Catalog.API.Entities.ProductImage;

namespace Catalog.API.Endpoints.ProductImages;

public class CreateProductImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/ProductImages", Handler)
            .WithName("CreateProductImage").WithSummary("Create a new ProductImage.")
            .WithTags(nameof(ProductImage))
            .Produces<ProductImageBaseResponseModel>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)));
    }

    private static async Task<Results<Created<ProductImageBaseResponseModel>, BadRequest<ProblemDetails>>> Handler(
        ProductImageBaseRequestModel requestModel,
        IProductImageRepository productImageRepository,
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

        var productImageModel = mapper.Map<ProductImage>(requestModel);
        var createdProductImage = await productImageRepository.Create(productImageModel);
        if (createdProductImage is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        return TypedResults.Created($"/api/ProductImages/{createdProductImage.Id}",
            mapper.Map<ProductImageBaseResponseModel>(createdProductImage));
    }
}
