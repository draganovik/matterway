using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Storage;

namespace Matterway.Catalog.Api.Features.ProductImages.Add;

public static class AddProductImageExtensions
{
    extension(ProductImage)
    {
        public static ProductImage FromRequest(AddProductImageRequest request, ImageStorageUploadResult uploadResult)
        {
            return new ProductImage
            {
                Id = uploadResult.ImageId,
                ProductId = request.ProductId,
                OrderIndex = request.OrderIndex,
                ImageUrl = uploadResult.ImageUrl,
                ImageAlt = request.ImageAlt ?? string.Empty,
            };
        }
    }

    extension(ProductImage entity)
    {
        public AddProductImageResponse ToResponse()
        {
            return new AddProductImageResponse
            {
                Id = entity.Id,
                OrderIndex = entity.OrderIndex,
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Title,
                ImageUrl = entity.ImageUrl,
                ImageAlt = entity.ImageAlt,
            };
        }
    }
}