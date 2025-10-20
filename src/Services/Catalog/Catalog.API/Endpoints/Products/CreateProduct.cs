using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models;
using Product = Catalog.API.Entities.Product;

namespace Catalog.API.Endpoints.Products;

public class CreateProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/Products", Handler)
            .WithName("CreateProduct").WithSummary("Create a new Product.")
            .WithTags("Products")
            .Produces<ProductBaseResponseModel>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)));
    }

    private static async Task<Results<Created<ProductBaseResponseModel>, BadRequest<ProblemDetails>>> Handler(
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

        return TypedResults.Created($"/api/Products/{createdProduct.Id}",
            mapper.Map<ProductBaseResponseModel>(createdProduct));
    }
}
