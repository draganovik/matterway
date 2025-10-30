using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.Customers.Contracts;

public class CustomerBaseRequest
{
    [Required]
    public Guid SystemUserId { get; set; }

    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    [Required]
    public DateTime BirthDate { get; set; }

    public Guid? DefaultAddressId { get; set; }
}