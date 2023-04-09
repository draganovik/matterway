using Customers.API.Entities;
using Customers.API.Models.CartItemModels;

namespace Customers.API.Repository;

public interface ICartItemRepository
{
    Task<ICollection<CartItem>> Query();

    Task<CartItem?> GetById(Guid id);

    Task<CartItem?> Create(CartItem requestModel);

    Task<CartItem?> Update(Guid id, CartItemUpdateRequestModel requestModel);

    Task<bool> Delete(Guid id);
}
