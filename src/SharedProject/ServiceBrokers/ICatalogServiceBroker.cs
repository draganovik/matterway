using Shared.Models;

namespace Shared.ServiceBrokers;

public interface ICatalogServiceBroker
{
    Task<Product?> GetProductById(Guid id);
}
