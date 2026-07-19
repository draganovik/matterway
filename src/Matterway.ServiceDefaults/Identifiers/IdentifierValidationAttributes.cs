using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Matterway.ServiceDefaults.Identifiers;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class ArticleCodeAttribute : ValidationAttribute
{
    public ArticleCodeAttribute()
    {
        ErrorMessage = $"The field {{0}} must contain exactly {ArticleCode.Length} letters or numbers.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        return value is string text && ArticleCode.TryParse(text, CultureInfo.InvariantCulture, out _)
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class ArticleCodeCollectionAttribute : ValidationAttribute
{
    public ArticleCodeCollectionAttribute()
    {
        ErrorMessage = $"Each article code in {{0}} must contain exactly {ArticleCode.Length} letters or numbers.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (value is IEnumerable<string?> codes &&
            codes.All(static code => ArticleCode.TryParse(code, CultureInfo.InvariantCulture, out _)))
            return ValidationResult.Success;

        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class OrderIdAttribute : ValidationAttribute
{
    public OrderIdAttribute()
    {
        ErrorMessage = $"The field {{0}} must contain {OrderId.Length} hexadecimal characters.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        return value is string text && OrderId.TryParse(text, CultureInfo.InvariantCulture, out _)
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    }
}
