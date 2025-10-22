namespace Common.Infrastructure.ServiceBrokers;

public interface ICustomersServiceBroker
{
    Task<bool> VerifyByCustomerId(Guid customerId);
    Task<Guid?> VerifyBySystemUserId(Guid systemUserId);
}
