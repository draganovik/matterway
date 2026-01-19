using Matterway.Sales.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Sales.Api.Providers.Persistence.PaymentEntity;

public sealed class EfPgPaymentRepository(SalesDbComposer context) : IPaymentRepository
{
    public async Task<Payment?> Create(Payment requestModel, CancellationToken cancellationToken = default)
    {
        context.Payment.Add(requestModel);
        var affected = await context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            return await context.Payment
                .AsNoTracking()
                .FirstOrDefaultAsync(payment => payment.Id == requestModel.Id, cancellationToken);

        return null;
    }

    public async Task<Payment?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Payment
            .AsNoTracking()
            .FirstOrDefaultAsync(payment => payment.Id == id, cancellationToken);
    }

    public async Task<ICollection<Payment>> Query(int pageIndex, int pageSize, Guid? orderId,
        CancellationToken cancellationToken = default)
    {
        var query = context.Payment
            .AsNoTracking();

        if (orderId.HasValue)
            query = query.Where(payment => payment.OrderId == orderId);

        return await query
            .OrderByDescending(payment => payment.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> Count(Guid? orderId, CancellationToken cancellationToken = default)
    {
        var query = context.Payment.AsQueryable();
        if (orderId.HasValue)
            query = query.Where(payment => payment.OrderId == orderId);

        return await query.CountAsync(cancellationToken);
    }
}