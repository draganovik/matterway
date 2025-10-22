using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Catalog.API.Features.ProductImages.Contracts;
using Catalog.API.Features.ProductImages.Data;
using Catalog.API.Features.ProductImages.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Iterfaces;

namespace Catalog.API.Features.ProductImages.Endpoints;

public class UpdateProductImageById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("ProductImages/{productId:guid}/{id:int}", Handler)
            .WithName("UpdateProductImageById").WithSummary("Update a ProductImage by id.")
            .WithTags(nameof(ProductImage))
            .Produces<ProductImageBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductImageBaseResponse>, NotFound, BadRequest<ProblemDetails>>> Handler(
        Guid productId,
        int id,
        ProductImageBaseRequest request,
        IProductImageRepository productImageRepository,
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

        var updatedProductImage = await productImageRepository.Update(productId, id, request);
        return updatedProductImage is not null
            ? TypedResults.Ok(mapper.Map<ProductImageBaseResponse>(updatedProductImage))
            : TypedResults.NotFound();
    }
}