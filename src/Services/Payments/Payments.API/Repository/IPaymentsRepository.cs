using Payments.API.Entities;
using Payments.API.Models.PaymentModels;

namespace Payments.API.Repository;

public interface IPaymentRepository
{
    Task<ICollection<Payment>> Query(int pageIndex, int pageSize);

    Task<Payment?> GetById(Guid id);

    Task<Payment?> Create(Payment requestModel);

    Task<Payment?> Update(Guid id, PaymentBaseRequestModel requestModel);

    Task<bool> Delete(Guid id);

    Task<int> GetTotalEntities();
}