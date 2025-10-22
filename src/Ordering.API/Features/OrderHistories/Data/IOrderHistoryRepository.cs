using Ordering.Api.Features.OrderHistories.Contracts;
using Ordering.Api.Features.OrderHistories.Domain;

namespace Ordering.Api.Features.OrderHistories.Data;

public interface IOrderHistoryRepository
{
    Task<ICollection<OrderHistory>> Query(int pageIndex, int pageSize);

    Task<OrderHistory?> GetById(Guid id);

    Task<OrderHistory?> Create(OrderHistory requestModel);

    Task<OrderHistory?> Update(Guid id, OrderHistoryBaseRequest requestModel);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}