using Microsoft.EntityFrameworkCore;
using Catalog.API.Data;
using Catalog.API.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace Catalog.API.Endpoints;

public static class ProductDetailEndpoints
{
    public static void MapProductDetailEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/ProductDetail").WithTags(nameof(ProductDetail));

        group.MapGet("/", async (CatalogDbContext db) =>
        {
            return await db.ProductDetail.ToListAsync();
        })
        .WithName("GetAllProductDetails")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<ProductDetail>, NotFound>> (Guid id, CatalogDbContext db) =>
        {
            return await db.ProductDetail.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is ProductDetail model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetProductDetailById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, ProductDetail productDetail, CatalogDbContext db) =>
        {
            var affected = await db.ProductDetail
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, productDetail.Id)
                  .SetProperty(m => m.ProductId, productDetail.ProductId)
                  .SetProperty(m => m.Type, productDetail.Type)
                  .SetProperty(m => m.Title, productDetail.Title)
                  .SetProperty(m => m.Value, productDetail.Value)
                  .SetProperty(m => m.Unit, productDetail.Unit)
                );

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateProductDetail")
        .WithOpenApi();

        group.MapPost("/", async (ProductDetail productDetail, CatalogDbContext db) =>
        {
            db.ProductDetail.Add(productDetail);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/ProductDetail/{productDetail.Id}",productDetail);
        })
        .WithName("CreateProductDetail")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, CatalogDbContext db) =>
        {
            var affected = await db.ProductDetail
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteProductDetail")
        .WithOpenApi();
    }
}
