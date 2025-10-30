using Catalog.Api.Domain;
using Catalog.Api.Features.ProductImages.Contracts;

namespace Catalog.Api.Features.ProductImages.Mapping;

public static class ProductImageMapperExtensions
{
    extension(ProductImage)
    {
        public static ProductImage FromContract(ProductImageBaseRequest request)
        {
            return new ProductImage
            {
                Id = request.Id,
                ProductId = request.ProductId,
                ImageUrl = request.ImageUrl,
                ImageAlt = request.ImageAlt,
                IsMain = request.IsMain
            };
        }
    }

    extension(ProductImage entity)
    {
        public T ToContract<T>() where T : class
        {
            if (typeof(T) == typeof(ProductImageBaseResponse))
            {
                return (T)(object)new ProductImageBaseResponse
                {
                    Id = entity.Id,
                    ProductId = entity.ProductId,
                    ProductName = entity.Product?.Title,
                    ImageUrl = entity.ImageUrl,
                    ImageAlt = entity.ImageAlt,
                    IsMain = entity.IsMain
                };
            }

            if (typeof(T) == typeof(ProductImageProductResponse))
            {
                return (T)(object)new ProductImageProductResponse
                {
                    Id = entity.Id,
                    ImageUrl = entity.ImageUrl,
                    ImageAlt = entity.ImageAlt,
                    IsMain = entity.IsMain
                };
            }

            throw new NotSupportedException($"Unsupported contract type: {typeof(T).FullName}");
        }
    }
}