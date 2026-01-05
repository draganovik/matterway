namespace Matterway.Customers.Api.Domain.Entities;

public class CartItem
{
    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? ProductName { get; set; }

    public Guid ProductId { get; set; }
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
}