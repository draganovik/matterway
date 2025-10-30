using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Features.ProductImages.Add;

public static class AddProductImageExtensions
{
    extension(ProductImage)
    {
        public static ProductImage FromRequest(AddProductImageRequest request)
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
        public AddProductImageResponse ToResponse()
        {
            return new AddProductImageResponse
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Title,
                ImageUrl = entity.ImageUrl,
                ImageAlt = entity.ImageAlt,
                IsMain = entity.IsMain
            };
        }
    }
}