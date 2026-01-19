using Matterway.Sales.Api.Domain.Entities;

namespace Matterway.Sales.Api.Features.Payments.RegisterPayment;

public static class RegisterPaymentMapper
{
    public static PaymentResponse MapToResponse(Payment entity)
    {
        return new PaymentResponse
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Provider = entity.Provider,
            ReferenceId = entity.ReferenceId,
            Amount = entity.Amount,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt
        };
    }
}