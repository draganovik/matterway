namespace Matterway.Customers.Api.Infrastructure.Brokers.Catalog;

public interface ICatalogClient
{
    Task<BrokerResponse<CatalogClientGetArticleResponse>> GetArticleByCode(ArticleCode code,
        CancellationToken cancellationToken);
}

public record CatalogClientGetArticleResponse
{
    public ArticleCode Code { get; init; }
    public string? Title { get; init; }
    public decimal? BasePrice { get; init; }
    public decimal? Price { get; init; }
}