namespace Shared.Models;

public class Customer
{
    public Guid Id { get; set; }
    public Guid SystemUserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public Guid DefaultAddressId { get; set; }
}