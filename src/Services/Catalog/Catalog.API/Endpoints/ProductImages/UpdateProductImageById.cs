using System;
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

public class UpdateProductImageById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/ProductImages/{productId:guid}/{id:int}", Handler)
            .WithName("UpdateProductImageById").WithSummary("Update a ProductImage by id.")
            .WithTags(nameof(ProductImage))
            .Produces<ProductImageBaseResponseModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)));
    }

    private static async Task<Results<Ok<ProductImageBaseResponseModel>, NotFound, BadRequest<ProblemDetails>>> Handler(
        Guid productId,
        int id,
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

        var updatedProductImage = await productImageRepository.Update(productId, id, requestModel);
        return updatedProductImage is not null
            ? TypedResults.Ok(mapper.Map<ProductImageBaseResponseModel>(updatedProductImage))
            : TypedResults.NotFound();
    }
}
