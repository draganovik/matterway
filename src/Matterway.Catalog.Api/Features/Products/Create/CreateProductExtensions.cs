using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Features.Products.Create;

public static class CreateProductExtensions
{
    extension(Product)
    {
        public static Product FromRequest(CreateProductRequest request)
        {
            return new Product
            {
                ProductCode = request.ProductCode,
                Title = request.Title,
                Price = request.Price,
                Description = request.Description,
                IsAvailable = request.IsAvailable
            };
        }
    }

    extension(Product entity)
    {
        public CreateProductResponse ToResponse()
        {
            return new CreateProductResponse
            {
                Id = entity.Id,
                ProductCode = entity.ProductCode,
                Title = entity.Title,
                Price = entity.Price,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                IsAvailable = entity.IsAvailable
            };
        }
    }
}