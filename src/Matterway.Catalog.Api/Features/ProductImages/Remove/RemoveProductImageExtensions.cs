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
                ProductId = entity.ProductId,
                ImageUrl = entity.ImageUrl ?? string.Empty
            };
        }
    }
}