using Matterway.Sales.Api.Domain.Entities;

namespace Matterway.Sales.Api.Providers.Persistence.OrderStatusEntity;

public sealed class EfPgOrderStatusRepository(SalesDbComposer context) : IOrderStatusRepository
{
    public async Task<OrderStatus?> Create(OrderStatus requestModel, CancellationToken cancellationToken = default)
    {
        context.OrderStatus.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        return affected > 0 ? requestModel : null;
    }
}