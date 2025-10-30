using Matterway.Customers.Api.Features.CartItems.Domain;

namespace Matterway.Customers.Api.Features.CartItems.Data;

public interface ICartItemRepository
{
    Task<ICollection<CartItem>> Query(int pageIndex, int pageSize);

    Task<ICollection<CartItem>> QueryByCustomerId(Guid systemUserId, int pageIndex, int pageSize);

    Task<CartItem?> GetById(Guid id, Guid productId);

    Task<CartItem?> Create(CartItem requestModel);

    Task<CartItem?> Put(CartItem requestModel);

    Task<bool> Delete(Guid id, Guid productId);

    Task<int> GetTotalEntities(Guid systemUserId);

    Task<int> GetTotalEntities();
}