namespace Matterway.Customers.Api.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public Guid SystemUserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
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