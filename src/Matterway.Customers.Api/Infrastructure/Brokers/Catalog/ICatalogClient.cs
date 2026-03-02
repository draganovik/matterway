namespace Matterway.Customers.Api.Infrastructure.Brokers.Catalog;

public interface ICatalogClient
{
    Task<BrokerResponse<CatalogClientGetArticleByIdResponse>> GetArticleById(Guid id,
        CancellationToken cancellationToken);
}

public record CatalogClientGetArticleByIdResponse
{
    public Guid Id { get; init; }
    public string? Code { get; init; }
    public string? Title { get; init; }
    public decimal? BasePrice { get; init; }
    public decimal? Price { get; init; }
}