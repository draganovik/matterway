using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductImageModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using SharedProject.ModelTemplates;
using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Endpoints;

public static class ProductImageEndpoints
{
    public static void MapProductImageEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/ProductImages").WithTags(nameof(ProductImage));

        group.MapGet("/", QueryProductImages)
            .WithName("QueryProductImages").WithOpenApi(operation => new(operation)
            {
                Summary = "Query ProductImages."
            });

        group.MapGet("/{productId}/{id}", GetProductImageById)
            .WithName("GetProductImageById").WithOpenApi(operation => new(operation)
            {
                Summary = "Get a ProductImage by id."
            });

        group.MapPatch("/{productId}/{id}", UpdateProductImageById)
            .WithName("UpdateProductImageById").WithOpenApi(operation => new(operation)
            {
                Summary = "Update a ProductImage by id."
            });

        group.MapPost("/", CreateProductImage)
            .WithName("CreateProductImage").WithOpenApi(operation => new(operation)
            {
                Summary = "Create a new ProductImage."
            });

        group.MapDelete("/{productId}/{id}", DeleteProductImage)
            .WithName("DeleteProductImage").WithOpenApi(operation => new(operation)
            {
                Summary = "Delete a ProductImage by id."
            });
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<PaginationResponse<ProductImageBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>> QueryProductImages([FromQuery] int page, [FromQuery] int pageSize, HttpContext httpContext, IProductImageRepository productImageRepository, IMapper mapper)
    {
        if (page < 1 || pageSize < 1)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid page or pageSize.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Page and pageSize must be greater than zero."
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

        var total = await productImageRepository.GetTotalEntities();
        var entities = await productImageRepository.Query(page, pageSize);
        var baseUri = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/Products");

        var paginationResponse = new PaginationResponse<ProductImageBaseResponseModel>(total, page, pageSize, mapper.Map<IEnumerable<ProductImageBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<ProductImage> value && value.Any()
                ? TypedResults.Ok(paginationResponse)
                : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<ProductImageBaseResponseModel>, NotFound>> GetProductImageById(Guid productId, int id, IProductImageRepository productImageRepository, IMapper mapper)
    {
        return await productImageRepository.GetById(productId, id)
            is ProductImage value
                ? TypedResults.Ok(mapper.Map<ProductImageBaseResponseModel>(value))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<ProductImageBaseResponseModel>, NotFound, BadRequest<ProblemDetails>>> UpdateProductImageById(Guid productId, int id, ProductImageBaseRequestModel requestModel, IProductImageRepository productImageRepository, IMapper mapper)
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
                Detail = "One or more validation errors occurred."
            };
            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var updatedUser = await productImageRepository.Update(productId, id, requestModel);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<ProductImageBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<ProductImageBaseResponseModel>, BadRequest<ProblemDetails>>> CreateProductImage(ProductImageBaseRequestModel requestModel, IProductImageRepository productImageRepository, IMapper mapper)
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
        return TypedResults.Created($"/api/ProductImages/{createdProductImage.Id}", mapper.Map<ProductImageBaseResponseModel>(createdProductImage));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteProductImage(Guid productId, int id, IProductImageRepository productImageRepository, IMapper mapper)
    {
        var isDeleted = await productImageRepository.Delete(productId, id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
