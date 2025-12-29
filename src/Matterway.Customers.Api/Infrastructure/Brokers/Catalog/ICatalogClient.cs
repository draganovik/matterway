namespace Matterway.Customers.Api.Infrastructure.Brokers.Catalog;

public interface ICatalogClient
{
    Task<CatalogClientGetProductByIdResponse?> GetProductById(Guid id, CancellationToken cancellationToken);
}

public record CatalogClientGetProductByIdResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public double BasePrice { get; init; }
    public double Price { get; init; }
}