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
                OrderIndex = entity.OrderIndex,
                ProductId = entity.ProductId,
                ImageUrl = entity.ImageUrl,
                ImageAlt = entity.ImageAlt
            };
        }
    }
}
