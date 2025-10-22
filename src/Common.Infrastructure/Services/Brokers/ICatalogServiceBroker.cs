using Common.Infrastructure.Models;

namespace Common.Infrastructure.Services.Brokers;

public interface ICatalogServiceBroker
{
    Task<Product?> GetProductById(Guid id);
}
