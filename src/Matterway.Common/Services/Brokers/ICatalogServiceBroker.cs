using Matterway.Common.Models;

namespace Matterway.Common.Services.Brokers;

public interface ICatalogServiceBroker
{
    Task<Product?> GetProductById(Guid id);
}