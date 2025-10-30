using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Features.ProductDetails.Add;

public static class AddProductDetailExtensions
{
    extension(ProductDetail)
    {
        public static ProductDetail FromRequest(AddProductDetailRequest request)
        {
            return new ProductDetail
            {
                ProductId = request.ProductId,
                Type = request.Type,
                Title = request.Title,
                Value = request.Value,
                Unit = request.Unit
            };
        }
    }

    extension(ProductDetail entity)
    {
        public AddProductDetailResponse ToResponse()
        {
            return new AddProductDetailResponse
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