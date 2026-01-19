namespace Matterway.Customers.Api.Domain.Entities;

public class CustomerArticle
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? ProductName { get; set; }

    public Guid ProductId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? OrderId { get; set; }

    public Customer? Customer { get; set; }
    public CustomerOrder? Order { get; set; }
}