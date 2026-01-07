using Matterway.Customers.Api.Domain.Entities;

namespace Matterway.Customers.Api.Providers.Persistence.CartItemEntity;

public interface ICartItemRepository
{
    Task<ICollection<CartItem>> QueryForSuid(Guid systemUserId, int pageIndex, int pageSize,
        CancellationToken cancellationToken = default);

    Task<CartItem?> Upsert(CartItem requestModel, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, Guid productId, CancellationToken cancellationToken = default);

    Task<CartItem?> GetBy(Guid id, Guid productId, CancellationToken cancellationToken = default);

    Task<int> Count(Guid systemUserId, CancellationToken cancellationToken = default);
}