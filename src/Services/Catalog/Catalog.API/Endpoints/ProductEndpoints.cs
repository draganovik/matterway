using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Enums;
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

        group.MapPut("/{id}", UpdateProductById)
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

    public static async Task<Results<Ok<IEnumerable<ProductBaseResponseModel>>, NoContent>> QueryProducts(IProductRepository productRepository, IMapper mapper)
    {
        return await productRepository.Query()
            is IEnumerable<Product> value && value.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<ProductBaseResponseModel>>(value))
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
    public static async Task<Results<Ok<ProductBaseResponseModel>, NotFound, BadRequest<object>>> UpdateProductById(Guid id, ProductBaseRequestModel requestModel, IProductRepository productRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var updatedUser = await productRepository.Update(id, requestModel);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<ProductBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<ProductBaseResponseModel>, BadRequest<object>>> CreateProduct(ProductBaseRequestModel requestModel, IProductRepository productRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var productModel = mapper.Map<Product>(requestModel);
        var createdProduct = await productRepository.Create(productModel);
        if (createdProduct is null)
        {
            return TypedResults.BadRequest<object>(new { message = "Cannot create entity" });
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
