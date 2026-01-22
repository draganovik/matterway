using Matterway.Customers.Api.Domain.Entities;

namespace Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

public interface ICustomerArticleRepository
{
    Task<ICollection<CustomerArticle>> QueryCart(Guid customerId, int pageIndex, int pageSize,
        CancellationToken cancellationToken = default);

    Task<ICollection<CustomerArticle>> GetCartItems(Guid customerId, CancellationToken cancellationToken = default);

    Task<CustomerArticle?> UpsertCartItem(CustomerArticle requestModel, CancellationToken cancellationToken = default);

    Task<bool> DeleteCartItem(Guid customerId, Guid articleId, CancellationToken cancellationToken = default);

    Task<CustomerArticle?> GetCartItem(Guid customerId, Guid articleId, CancellationToken cancellationToken = default);

    Task<int> CountCart(Guid customerId, CancellationToken cancellationToken = default);

    Task<ICollection<CustomerArticle>> QueryByOrderId(Guid orderId, CancellationToken cancellationToken = default);
}