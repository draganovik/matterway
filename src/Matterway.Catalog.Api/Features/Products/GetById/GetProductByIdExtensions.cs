using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Features.Products.GetById;

public static class GetProductByIdMapping
{
    extension(Product entity)
    {
        public GetProductByIdResponse ToResponse()
        {
            return new GetProductByIdResponse
            {
                Id = entity.Id,
                ProductCode = entity.ProductCode,
                Title = entity.Title,
                Price = entity.Price,
                Description = entity.Description,
                ProductDetails = entity.ProductDetails?
                    .Select(pd => pd.ToResponse()).ToList(),
                ProductImages = entity.ProductImages?
                    .OrderBy(pi => pi.Id)
                    .Select(pi => pi.ToResponse()).ToList(),
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                IsAvailable = entity.IsAvailable
            };
        }
    }

    extension(ProductDetail entity)
    {
        public ProductDetailProperty ToResponse()
        {
            return new ProductDetailProperty
            {
                Id = entity.Id,
                Type = entity.Type,
                Title = entity.Title,
                Value = entity.Value,
                Unit = entity.Unit
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
                ImageRef = entity.ImageRef,
                ImageUrl = entity.ImageUrl,
                ImageAlt = entity.ImageAlt
            };
        }
    }
}
