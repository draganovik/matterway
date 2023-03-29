using Identity.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Entities;

public class SystemUser : IValidatableObject
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string PasswordHash { get; set; } = string.Empty;

    public Session? Session { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public SystemUserRole Role { get; set; } = SystemUserRole.Customer;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (!string.IsNullOrWhiteSpace(Email) && !new EmailAddressAttribute().IsValid(Email))
        {
            results.Add(new ValidationResult("Invalid email format."));
        }

        return results;
    }
}
