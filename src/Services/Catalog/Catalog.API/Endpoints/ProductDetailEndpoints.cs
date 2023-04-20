using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductDetailModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Endpoints;

public static class ProductDetailEndpoints
{
    public static void MapProductDetailEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/ProductDetails").WithTags(nameof(ProductDetail));

        group.MapGet("/", QueryProductDetails)
            .WithName("QueryProductDetails").WithOpenApi(operation => new(operation)
            {
                Summary = "Query ProductDetails."
            });

        group.MapGet("/{id}", GetProductDetailById)
            .WithName("GetProductDetailById").WithOpenApi(operation => new(operation)
            {
                Summary = "Get a ProductDetail by id."
            });

        group.MapPatch("/{id}", UpdateProductDetailById)
            .WithName("UpdateProductDetailById").WithOpenApi(operation => new(operation)
            {
                Summary = "Update a ProductDetail by id."
            });

        group.MapPost("/", CreateProductDetail)
            .WithName("CreateProductDetail").WithOpenApi(operation => new(operation)
            {
                Summary = "Create a new ProductDetail."
            });

        group.MapDelete("/{id}", DeleteProductDetail)
            .WithName("DeleteProductDetail").WithOpenApi(operation => new(operation)
            {
                Summary = "Delete a ProductDetail by id."
            });
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<ProductDetailBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>> QueryProductDetails([FromQuery] int page, [FromQuery] int pageSize, IProductDetailRepository productDetailRepository, IMapper mapper)
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
        return await productDetailRepository.Query(page, pageSize)
            is IEnumerable<ProductDetail> value && value.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<ProductDetailBaseResponseModel>>(value))
                : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<ProductDetailBaseResponseModel>, NotFound>> GetProductDetailById(Guid id, IProductDetailRepository productDetailRepository, IMapper mapper)
    {
        return await productDetailRepository.GetById(id)
            is ProductDetail value
                ? TypedResults.Ok(mapper.Map<ProductDetailBaseResponseModel>(value))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<ProductDetailBaseResponseModel>, NotFound, BadRequest<object>>> UpdateProductDetailById(Guid id, ProductDetailBaseRequestModel requestModel, IProductDetailRepository productDetailRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }
        var updatedUser = await productDetailRepository.Update(id, requestModel);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<ProductDetailBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<ProductDetailBaseResponseModel>, BadRequest<object>>> CreateProductDetail(ProductDetailBaseRequestModel requestModel, IProductDetailRepository productDetailRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var productDetailModel = mapper.Map<ProductDetail>(requestModel);
        var createdProductDetail = await productDetailRepository.Create(productDetailModel);
        if (createdProductDetail is null)
        {
            return TypedResults.BadRequest<object>(new { message = "Cannot create entity" });
        }
        return TypedResults.Created($"/api/ProductDetails/{createdProductDetail.Id}", mapper.Map<ProductDetailBaseResponseModel>(createdProductDetail));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteProductDetail(Guid id, IProductDetailRepository productDetailRepository, IMapper mapper)
    {
        var isDeleted = await productDetailRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
