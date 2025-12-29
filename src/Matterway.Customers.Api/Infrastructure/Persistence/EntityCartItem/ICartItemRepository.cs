using Matterway.Customers.Api.Domain.Entities;

namespace Matterway.Customers.Api.Infrastructure.Persistence.EntityCartItem;

public interface ICartItemRepository
{
    Task<ICollection<CartItem>> QueryForSystemUserId(Guid systemUserId, int pageIndex, int pageSize);

    Task<CartItem?> Upsert(CartItem requestModel);

    Task<bool> Delete(Guid id, Guid productId);

    Task<CartItem?> GetById(Guid id, Guid productId);

    Task<int> Count(Guid systemUserId);
}