using Matterway.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Payments.Api.Features.Payments.Contracts;

public class PaymentBaseResponse
{
    public Guid Id { get; set; }

    [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}$",
        ErrorMessage = "Invalid ReferenceNumber. ReferenceNumber format must be: 0000-0000-0000-0000")]
    public string? ReferenceNumber { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public double PaymentAmount { get; set; }

    [RegularExpression(@"^[*]{4}-[*]{4}-[*]{4}-[0-9]{4}$",
        ErrorMessage = "Invalid CardNumber. CardNumber format must be: ****-****-****-0000")]
    public string? CardNumber { get; set; }

    public string? CardHolder { get; set; }

    [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2}|[0-9]{2})$",
        ErrorMessage = "Invalid ExpirationDate. ExpirationDate format must be: MM/YY")]
    public string? ExpirationDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentState PaymentState { get; set; }
}