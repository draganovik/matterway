using Catalog.Api.Domain;
using Catalog.Api.Features.ProductDetails.Contracts;

namespace Catalog.Api.Features.ProductDetails.Mapping;

public static class ProductDetailMapperExtensions
{
    extension(ProductDetail)
    {
        public static ProductDetail FromContract(ProductDetailBaseRequest request)
        {
            return new ProductDetail
            {
                ProductId = request.ProductId,
                Type = request.Type,
                Title = request.Title,
                Value = request.Value,
                Unit = request.Unit
            };
        }
    }

    extension(ProductDetail entity)
    {
        public T ToContract<T>() where T : class
        {
            if (typeof(T) == typeof(ProductDetailBaseResponse))
            {
                return (T)(object)new ProductDetailBaseResponse
                {
                    Id = entity.Id,
                    ProductId = entity.ProductId,
                    Type = entity.Type,
                    Title = entity.Title,
                    Value = entity.Value,
                    Unit = entity.Unit
                };
            }

            if (typeof(T) == typeof(ProductDetailProductResponse))
            {
                return (T)(object)new ProductDetailProductResponse
                {
                    Id = entity.Id,
                    Type = entity.Type,
                    Title = entity.Title,
                    Value = entity.Value,
                    Unit = entity.Unit
                };
            }

            throw new NotSupportedException($"Unsupported contract type: {typeof(T).FullName}");
        }
    }
}