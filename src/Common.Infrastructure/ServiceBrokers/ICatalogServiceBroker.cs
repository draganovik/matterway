using Common.Infrastructure.Models;

namespace Common.Infrastructure.ServiceBrokers;

public interface ICatalogServiceBroker
{
    Task<Product?> GetProductById(Guid id);
}
