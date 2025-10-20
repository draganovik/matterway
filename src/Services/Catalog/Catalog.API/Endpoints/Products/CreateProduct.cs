using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Catalog.API.Helpers;
using Catalog.API.Models.ProductModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Iterfaces;
using Product = Catalog.API.Entities.Product;

namespace Catalog.API.Endpoints.Products;

public class CreateProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Products", Handler)
            .WithName("CreateProduct").WithSummary("Create a new Product.")
            .WithTags("Products")
            .Produces<ProductBaseResponseModel>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<ProductBaseResponseModel>, BadRequest<ProblemDetails>>> Handler(
        ProductBaseRequestModel requestModel,
        HttpContext httpContext,
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

        var productModel = mapper.Map<Product>(requestModel);
        var createdProduct = await productRepository.Create(productModel);
        if (createdProduct is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var location = ResourceUrlHelper.BuildResourceLocation(httpContext, $"Products/{createdProduct.Id}");

        return TypedResults.Created(location,
            mapper.Map<ProductBaseResponseModel>(createdProduct));
    }
}