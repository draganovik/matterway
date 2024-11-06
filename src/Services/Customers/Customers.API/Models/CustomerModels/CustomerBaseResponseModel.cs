namespace Customers.API.Models.CustomerModels;

public class CustomerBaseResponseModel
{
    public Guid Id { get; set; }
    public Guid SystemUserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public Guid? DefaultAddressId { get; set; }
}