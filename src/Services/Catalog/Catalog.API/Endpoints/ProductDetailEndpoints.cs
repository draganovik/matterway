using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductDetailModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using SharedProject.ModelTemplates;
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
    public static async Task<Results<Ok<PaginationResponse<ProductDetailBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>> QueryProductDetails([FromQuery] int page, [FromQuery] int pageSize, HttpContext httpContext, IProductDetailRepository productDetailRepository, IMapper mapper)
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

        var total = await productDetailRepository.GetTotalEntities();
        var entities = await productDetailRepository.Query(page, pageSize);
        var baseUri = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/ProductDetails");

        var paginationResponse = new PaginationResponse<ProductDetailBaseResponseModel>(total, page, pageSize, mapper.Map<IEnumerable<ProductDetailBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<ProductDetail> value && value.Any()
                ? TypedResults.Ok(paginationResponse)
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
    public static async Task<Results<Ok<ProductDetailBaseResponseModel>, NotFound, BadRequest<ProblemDetails>>> UpdateProductDetailById(Guid id, ProductDetailBaseRequestModel requestModel, IProductDetailRepository productDetailRepository, IMapper mapper)
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
        var updatedUser = await productDetailRepository.Update(id, requestModel);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<ProductDetailBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<ProductDetailBaseResponseModel>, BadRequest<ProblemDetails>>> CreateProductDetail(ProductDetailBaseRequestModel requestModel, IProductDetailRepository productDetailRepository, IMapper mapper)
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

        var productDetailModel = mapper.Map<ProductDetail>(requestModel);
        var createdProductDetail = await productDetailRepository.Create(productDetailModel);
        if (createdProductDetail is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
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
