using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Features.ProductImages.Remove;

public static class RemoveProductImageExtensions
{
    extension(ProductImage entity)
    {
        public RemoveProductImageResponse ToResponse()
        {
            return new RemoveProductImageResponse
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                ImageUrl = entity.ImageUrl,
                ImageRef = entity.ImageRef,
                ImageAlt = entity.ImageAlt
            };
        }
    }
}
