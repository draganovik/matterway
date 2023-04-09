using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductDetailModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Enums;

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

        group.MapPut("/{id}", UpdateProductDetailById)
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
    public static async Task<Results<Ok<IEnumerable<ProductDetailBaseResponseModel>>, NoContent>> QueryProductDetails(IProductDetailRepository productDetailRepository, IMapper mapper)
    {
        return await productDetailRepository.Query()
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
    public static async Task<Results<Ok<ProductDetailBaseResponseModel>, NotFound>> UpdateProductDetailById(Guid id, ProductDetailBaseRequestModel requestModel, IProductDetailRepository productDetailRepository, IMapper mapper)
    {
        var updatedUser = await productDetailRepository.Update(id, requestModel);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<ProductDetailBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<ProductDetailBaseResponseModel>, BadRequest>> CreateProductDetail(ProductDetailBaseRequestModel requestModel, IProductDetailRepository productDetailRepository, IMapper mapper)
    {
        var productDetailModel = mapper.Map<ProductDetail>(requestModel);
        var createdProductDetail = await productDetailRepository.Create(productDetailModel);
        if (createdProductDetail is null)
        {
            return TypedResults.BadRequest();
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
