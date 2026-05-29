using System.ComponentModel.DataAnnotations;
using Matterway.Sales.Api.Domain;

namespace Matterway.Sales.Api.Features.Admin.Orders.Contracts;

public record AdminAddOrderStatusRequest
{
    [Required]
    public EOrderStatusType? Status { get; init; }

    public string? Note { get; init; }
}