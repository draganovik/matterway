using Catalog.Api.Domain;
using Catalog.Api.Features.ProductDetails.Contracts;
using Catalog.Api.Features.ProductDetails.Mapping;
using Catalog.Api.Features.ProductImages.Contracts;
using Catalog.Api.Features.ProductImages.Mapping;
using Catalog.Api.Features.Products.Contracts;

namespace Catalog.Api.Features.Products.Mapping;

public static class ProductMapperExtensions
{
    extension(Product)
    {
        public static Product FromContract(ProductBaseRequest request)
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
        public T ToContract<T>() where T : class
        {
            if (typeof(T) == typeof(ProductBaseResponse))
            {
                return (T)(object)new ProductBaseResponse
                {
                    Id = entity.Id,
                    ProductCode = entity.ProductCode,
                    Title = entity.Title,
                    Price = entity.Price,
                    Description = entity.Description,
                    ProductDetails = entity.ProductDetails?
                        .Select(pd => pd.ToContract<ProductDetailProductResponse>()).ToList() ?? [],
                    ProductImages = entity.ProductImages?
                        .Select(pi => pi.ToContract<ProductImageProductResponse>()).ToList() ?? [],
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt,
                    IsAvailable = entity.IsAvailable
                };
            }

            throw new NotSupportedException($"Unsupported contract type: {typeof(T).FullName}");
        }
    }
}