using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Domain.Entities;

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
    public DateOnly BirthDate { get; set; }

    public Guid? DefaultAddressId { get; set; }

    public void UpdateDetails(string? firstName, string? lastName, DateOnly? birthDate, Guid? defaultAddressId)
    {
        if (!string.IsNullOrWhiteSpace(firstName))
            FirstName = firstName;

        if (!string.IsNullOrWhiteSpace(lastName))
            LastName = lastName;

        if (birthDate.HasValue)
            BirthDate = birthDate.Value;

        if (defaultAddressId.HasValue)
            DefaultAddressId = defaultAddressId.Value;
    }
}