using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Features.ProductDetails.Update;

public static class UpdateProductDetailExtensions
{
    extension(ProductDetail entity)
    {
        public void MapUpdates(UpdateProductDetailRequest request)
        {
            entity.Type = request.Type;
            entity.Title = request.Title ?? entity.Title;
            entity.Value = request.Value ?? entity.Value;
            entity.Unit = request.Unit ?? entity.Unit;
        }

        public UpdateProductDetailResponse ToResponse()
        {
            return new UpdateProductDetailResponse
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                ProductTitle = entity.Product?.Title,
                Type = entity.Type,
                Title = entity.Title,
                Value = entity.Value,
                Unit = entity.Unit
            };
        }
    }
}