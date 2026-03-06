namespace Matterway.Customers.Api.Domain.Entities;

public class CustomerArticle
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? ArticleName { get; set; }
    public required string ArticleCode { get; set; }

    public Guid CustomerId { get; set; }
    public OrderId? OrderId { get; set; }

    public Customer? Customer { get; set; }
    public CustomerOrder? Order { get; set; }
}