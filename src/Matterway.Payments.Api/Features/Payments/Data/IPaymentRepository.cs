using Matterway.Payments.Api.Features.Payments.Contracts;
using Matterway.Payments.Api.Features.Payments.Domain;

namespace Matterway.Payments.Api.Features.Payments.Data;

public interface IPaymentRepository
{
    Task<ICollection<Payment>> Query(int pageIndex, int pageSize);

    Task<Payment?> GetById(Guid id);

    Task<Payment?> Create(Payment requestModel);

    Task<Payment?> Update(Guid id, PaymentBaseRequest requestModel);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}