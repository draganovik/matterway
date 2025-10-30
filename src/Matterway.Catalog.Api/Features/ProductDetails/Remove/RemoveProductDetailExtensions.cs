using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Features.ProductImages.Remove;

namespace Matterway.Catalog.Api.Features.ProductDetails.Remove;

public static class RemoveProductDetailExtensions
{
    extension(ProductDetail entity)
    {
        public RemoveProductDetailResponse ToResponse()
        {
            return new RemoveProductDetailResponse
            {
                DetailType = entity.Title ?? $"{entity.Type.ToString()}: {entity.Value}",
                ProductId = entity.ProductId,
            };
        }
    }
}