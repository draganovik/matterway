using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Catalog.API.Models.ProductModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Iterfaces;

namespace Catalog.API.Endpoints.Products;

public class UpdateProductById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Products/{id:guid}", Handler)
            .WithName("UpdateProductById").WithSummary("Update a Product by id.")
            .WithTags("Products")
            .Produces<ProductBaseResponseModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductBaseResponseModel>, NotFound, BadRequest<ProblemDetails>>> Handler(
        Guid id,
        ProductBaseRequestModel requestModel,
        IProductRepository productRepository,
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

        var updatedProduct = await productRepository.Update(id, requestModel);
        return updatedProduct is not null
            ? TypedResults.Ok(mapper.Map<ProductBaseResponseModel>(updatedProduct))
            : TypedResults.NotFound();
    }
}