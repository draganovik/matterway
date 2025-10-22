namespace Common.Infrastructure.Services.Brokers;

public interface ICustomersServiceBroker
{
    Task<bool> VerifyByCustomerId(Guid customerId);
    Task<Guid?> VerifyBySystemUserId(Guid systemUserId);
}