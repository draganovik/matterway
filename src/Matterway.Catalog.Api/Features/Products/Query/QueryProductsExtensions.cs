using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Features.Products.Query;

public static class QueryProductsExtensions
{
    extension(Product entity)
    {
        public QueryProductResponse ToResponse()
        {
            return new QueryProductResponse
            {
                Id = entity.Id,
                ProductCode = entity.ProductCode,
                Title = entity.Title,
                Price = entity.Price,
                Description = entity.Description,
                ThumbnailImage = entity.ProductImages?
                    .OrderBy(pi => pi.OrderIndex)
                    .Select(pi => pi.ToResponse())
                    .FirstOrDefault(),
                IsAvailable = entity.IsAvailable
            };
        }
    }

    extension(ProductImage entity)
    {
        public ProductImageProperty ToResponse()
        {
            return new ProductImageProperty
            {
                Id = entity.Id,
                OrderIndex = entity.OrderIndex,
                ImageUrl = entity.ImageUrl,
                ImageAlt = entity.ImageAlt
            };
        }
    }
}