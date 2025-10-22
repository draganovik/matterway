using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Customers.Api.Features.Customers.Domain;

[Index(nameof(SystemUserId), IsUnique = true)]
public class Customer
{
    [Key]
    public Guid Id { get; set; }

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