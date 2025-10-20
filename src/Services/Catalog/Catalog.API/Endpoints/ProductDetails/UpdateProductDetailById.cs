using System;
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

public class UpdateProductDetailById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/ProductDetails/{id:guid}", Handler)
            .WithName("UpdateProductDetailById").WithSummary("Update a ProductDetail by id.")
            .WithTags(nameof(ProductDetail))
            .Produces<ProductDetailBaseResponseModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)));
    }

    private static async Task<Results<Ok<ProductDetailBaseResponseModel>, NotFound, BadRequest<ProblemDetails>>> Handler(
        Guid id,
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

        var updatedProductDetail = await productDetailRepository.Update(id, requestModel);
        return updatedProductDetail is not null
            ? TypedResults.Ok(mapper.Map<ProductDetailBaseResponseModel>(updatedProductDetail))
            : TypedResults.NotFound();
    }
}
