using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Catalog.API.Models.ProductImageModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Iterfaces;
using ProductImage = Catalog.API.Entities.ProductImage;

namespace Catalog.API.Endpoints.ProductImages;

public class UpdateProductImageById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("ProductImages/{productId:guid}/{id:int}", Handler)
            .WithName("UpdateProductImageById").WithSummary("Update a ProductImage by id.")
            .WithTags(nameof(ProductImage))
            .Produces<ProductImageBaseResponseModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
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