using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Filters;
using Catalog.API.Models.ProductModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using SharedProject.ModelTemplates;
using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Products").WithTags(nameof(Product));

        group.MapGet("/", QueryProducts)
            .WithName("QueryProducts").WithOpenApi(operation => new(operation)
            {
                Summary = "Query Products."
            });

        group.MapGet("/{id}", GetProductById)
            .WithName("GetProductById").WithOpenApi(operation => new(operation)
            {
                Summary = "Get a Product by id."
            });

        group.MapPatch("/{id}", UpdateProductById)
            .WithName("UpdateProductById").WithOpenApi(operation => new(operation)
            {
                Summary = "Update a Product by id."
            });

        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct").WithOpenApi(operation => new(operation)
            {
                Summary = "Create a new Product."
            });

        group.MapDelete("/{id}", DeleteProduct)
            .WithName("DeleteProduct").WithOpenApi(operation => new(operation)
            {
                Summary = "Delete a Product by id."
            });
    }

    public static async Task<Results<Ok<PaginationResponse<ProductBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>> QueryProducts([FromQuery] int page, [FromQuery] int pageSize, [AsParameters] ProductFilter productFilter, HttpContext httpContext, IProductRepository productRepository, IMapper mapper)
    {
        if (page < 1 || pageSize < 1)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid page or pageSize.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Page and pageSize must be greater than zero.",
                Instance = httpContext.Request.Path
            };
            var results = new List<ValidationResult>();
            if (page < 1)
            {
                results.Add(new ValidationResult("Page must be greater than zero.", new[] { nameof(page) }));
            }
            if (pageSize < 1)
            {
                results.Add(new ValidationResult("PageSize must be greater than zero.", new[] { nameof(pageSize) }));
            }

            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var total = await productRepository.GetTotalEntities();
        var entities = await productRepository.Query(page, pageSize, productFilter);
        var baseUri = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/Products");

        var paginationResponse = new PaginationResponse<ProductBaseResponseModel>(total, page, pageSize, mapper.Map<IEnumerable<ProductBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<Product> value && value.Any()
                ? TypedResults.Ok(paginationResponse)
                : TypedResults.NoContent();
    }

    public static async Task<Results<Ok<ProductBaseResponseModel>, NotFound>> GetProductById(Guid id, IProductRepository productRepository, IMapper mapper)
    {
        return await productRepository.GetById(id)
            is Product value
                ? TypedResults.Ok(mapper.Map<ProductBaseResponseModel>(value))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<ProductBaseResponseModel>, NotFound, BadRequest<ProblemDetails>>> UpdateProductById(Guid id, ProductBaseRequestModel requestModel, HttpContext httpContext, IProductRepository productRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred.",
                Instance = httpContext.Request.Path
            };
            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var updatedUser = await productRepository.Update(id, requestModel);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<ProductBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<ProductBaseResponseModel>, BadRequest<ProblemDetails>>> CreateProduct(ProductBaseRequestModel requestModel, HttpContext httpContext, IProductRepository productRepository, IMapper mapper)
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
                Detail = "One or more validation errors occurred.",
                Instance = httpContext.Request.Path
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
                Detail = "Cannot create entity",
                Instance = httpContext.Request.Path
            };
            return TypedResults.BadRequest(problemDetails);
        }
        return TypedResults.Created($"/api/Products/{createdProduct.Id}", mapper.Map<ProductBaseResponseModel>(createdProduct));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteProduct(Guid id, IProductRepository productRepository, IMapper mapper)
    {
        var isDeleted = await productRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
