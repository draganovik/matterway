using System.ComponentModel.DataAnnotations;

namespace Identity.API.Entities;

public class Session : IValidatableObject
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "SystemUserId is required.")]
    public Guid? SystemUserId { get; set; }

    public SystemUser? SystemUser { get; set; }

    [Required(ErrorMessage = "Token is required.")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "RefreshToken is required.")]
    public string RefreshToken { get; set; } = string.Empty;

    public DateTime? Created { get; set; }

    [Required(ErrorMessage = "Expires is required.")]
    public DateTime? Expires { get; set; }

    [Required(ErrorMessage = "Refresh expires is required.")]
    public DateTime? RefreshExpires { get; set; }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= Expires;
    }

    public bool IsExpiredRefresh()
    {
        return DateTime.UtcNow >= Expires;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (SystemUserId == Guid.Empty)
        {
            results.Add(new ValidationResult("SystemUserId is required."));
        }

        if (string.IsNullOrEmpty(Token))
        {
            results.Add(new ValidationResult("Token is required."));
        }

        if (string.IsNullOrEmpty(RefreshToken))
        {
            results.Add(new ValidationResult("RefreshToken is required."));
        }

        return results;
    }
}
