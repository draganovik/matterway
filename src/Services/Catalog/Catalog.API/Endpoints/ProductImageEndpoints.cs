using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductImageModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Enums;

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

        group.MapPut("/{productId}/{id}", UpdateProductImageById)
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
    public static async Task<Results<Ok<IEnumerable<ProductImageBaseResponseModel>>, NoContent>> QueryProductImages(IProductImageRepository productImageRepository, IMapper mapper)
    {
        return await productImageRepository.Query()
            is IEnumerable<ProductImage> value && value.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<ProductImageBaseResponseModel>>(value))
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
    public static async Task<Results<Ok<ProductImageBaseResponseModel>, NotFound>> UpdateProductImageById(Guid productId, int id, ProductImageBaseRequestModel requestModel, IProductImageRepository productImageRepository, IMapper mapper)
    {
        var updatedUser = await productImageRepository.Update(productId, id, requestModel);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<ProductImageBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<ProductImageBaseResponseModel>, BadRequest>> CreateProductImage(ProductImageBaseRequestModel requestModel, IProductImageRepository productImageRepository, IMapper mapper)
    {
        var productImageModel = mapper.Map<ProductImage>(requestModel);
        var createdProductImage = await productImageRepository.Create(productImageModel);
        if (createdProductImage is null)
        {
            return TypedResults.BadRequest();
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
