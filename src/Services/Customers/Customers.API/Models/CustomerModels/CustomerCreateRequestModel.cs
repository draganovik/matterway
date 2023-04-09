using System.ComponentModel.DataAnnotations;

namespace Customers.API.Models.CustomerModels;

public class CustomerCreateRequestModel
{

    [Required]
    public Guid SystemUserId { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
}
