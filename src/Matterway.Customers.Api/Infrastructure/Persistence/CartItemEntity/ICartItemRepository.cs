using Matterway.Customers.Api.Domain.Entities;

namespace Matterway.Customers.Api.Infrastructure.Persistence.CartItemEntity;

public interface ICartItemRepository
{
    Task<ICollection<CartItem>> QueryForSuid(Guid systemUserId, int pageIndex, int pageSize);

    Task<CartItem?> Upsert(CartItem requestModel);

    Task<bool> Delete(Guid id, Guid productId);

    Task<CartItem?> GetBy(Guid id, Guid productId);

    Task<int> Count(Guid systemUserId);
}