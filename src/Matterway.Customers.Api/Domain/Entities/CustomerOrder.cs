namespace Matterway.Customers.Api.Domain.Entities;

public class CustomerOrder
{
    public OrderId OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;

    public Customer? Customer { get; set; }
    public List<CustomerArticle> Items { get; } = [];
}