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
                ImageUrl = entity.ImageUrl,
                ImageAlt = entity.ImageAlt
            };
        }
    }
}