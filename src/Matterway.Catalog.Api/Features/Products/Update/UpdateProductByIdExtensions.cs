using Matterway.Catalog.Api.Domain;

namespace Matterway.Catalog.Api.Features.Products.Update;

public static class UpdateProductExtensions
{
    extension(Product entity)
    {
        public void ApplyUpdate(UpdateProductByIdRequest request)
        {
            entity.ProductCode = request.ProductCode ?? entity.ProductCode;
            entity.Title = request.Title ?? entity.Title;
            entity.Price = request.Price ?? entity.Price;
            entity.Description = request.Description ?? entity.Description;
            entity.IsAvailable = request.IsAvailable;
            entity.UpdatedAt = DateTime.Now;
        }

        public UpdateProductByIdResponse ToResponse()
        {
            return new UpdateProductByIdResponse
            {
                Id = entity.Id,
                ProductCode = entity.ProductCode,
                Title = entity.Title,
                Price = entity.Price,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                IsAvailable = entity.IsAvailable
            };
        }
    }
}