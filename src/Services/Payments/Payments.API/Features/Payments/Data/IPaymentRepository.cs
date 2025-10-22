using Payments.API.Features.Payments.Contracts;
using Payments.API.Features.Payments.Domain;

namespace Payments.API.Features.Payments.Data;

public interface IPaymentRepository
{
    Task<ICollection<Payment>> Query(int pageIndex, int pageSize);

    Task<Payment?> GetById(Guid id);

    Task<Payment?> Create(Payment requestModel);

    Task<Payment?> Update(Guid id, PaymentBaseRequest requestModel);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}