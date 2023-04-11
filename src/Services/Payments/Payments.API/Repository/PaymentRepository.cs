using Microsoft.EntityFrameworkCore;
using Payments.API.Data;
using Payments.API.Entities;
using Payments.API.Models.PaymentModels;

namespace Payments.API.Repository;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentsDbContext context;

    public PaymentRepository(PaymentsDbContext context)
    {
        this.context = context;
    }

    public async Task<Payment?> Create(Payment requestModel)
    {
        context.Payment.Add(requestModel);
        var affected = await context.SaveChangesAsync();
        if (affected == 1)
        {
            return await context.Payment.FindAsync(requestModel.Id);
        }
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

    public async Task<ICollection<Payment>> Query()
    {
        return await context.Payment.AsNoTracking()
        .ToListAsync();
    }

    public async Task<Payment?> Update(Guid id, PaymentBaseRequestModel requestModel)
    {
        var currentPaymentModel = await context.Payment.FindAsync(id);
        if (currentPaymentModel is null)
        {
            return null;
        }
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
