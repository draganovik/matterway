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
                Id = request.Id,
                ProductId = request.ProductId,
                ImageRef = uploadResult.ImageRef,
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
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Title,
                ImageRef = entity.ImageRef,
                ImageUrl = entity.ImageUrl,
                ImageAlt = entity.ImageAlt,
            };
        }
    }
}
