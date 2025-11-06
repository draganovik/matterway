using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Features.ProductImages.Update;

public static class UpdateProductImageExtensions
{
    extension(ProductImage entity)
    {
        public void MapUpdates(UpdateProductImageRequest request)
        {
            entity.ImageUrl = request.ImageUrl ?? entity.ImageUrl;
            entity.ImageAlt = request.ImageAlt ?? entity.ImageAlt;
        }

        public UpdateProductImageResponse ToResponse()
        {
            return new UpdateProductImageResponse
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Title,
                ImageUrl = entity.ImageUrl,
                ImageAlt = entity.ImageAlt,
            };
        }
    }
}