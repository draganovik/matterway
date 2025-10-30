using Matterway.Payments.Api.Data;
using Matterway.Payments.Api.Features.Payments.Contracts;
using Matterway.Payments.Api.Features.Payments.Domain;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Payments.Api.Features.Payments.Data;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentsDb context;

    public PaymentRepository(PaymentsDb context)
    {
        this.context = context;
    }

    public async Task<Payment?> Create(Payment requestModel)
    {
        context.Payment.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1) return await context.Payment.FindAsync(requestModel.Id);
        return null;
    }

    public async Task<bool> Delete(Guid id)
    {
        var affected = await context.Payment
            .Where(model => model.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1;
    }

    public async Task<Payment?> GetById(Guid id)
    {
        return await context.Payment.FindAsync(id);
    }

    public Task<int> GetTotalEntities()
    {
        return context.Payment.CountAsync();
    }

    public async Task<ICollection<Payment>> Query(int pageIndex, int pageSize)
    {
        return await context.Payment.AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Payment?> Update(Guid id, PaymentBaseRequest requestModel)
    {
        var currentPaymentModel = await context.Payment.FindAsync(id);
        if (currentPaymentModel is null) return null;
        var affected = await context.Payment
            .Where(model => model.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.ReferenceNumber, requestModel.ReferenceNumber)
                .SetProperty(m => m.PaymentDate, requestModel.PaymentDate)
                .SetProperty(m => m.PaymentAmount, requestModel.PaymentAmount)
                .SetProperty(m => m.CardNumber, requestModel.CardNumber)
                .SetProperty(m => m.CardHolder, requestModel.CardHolder)
                .SetProperty(m => m.ExpirationDate, requestModel.ExpirationDate)
                .SetProperty(m => m.SecurityCode, requestModel.SecurityCode)
                .SetProperty(m => m.PaymentState, requestModel.PaymentState)
            );
        await context.Entry(currentPaymentModel).ReloadAsync();
        return affected == 1 ? currentPaymentModel : null;
    }
}