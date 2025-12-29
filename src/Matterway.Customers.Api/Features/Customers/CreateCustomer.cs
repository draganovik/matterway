using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Customers.Api.Application;
using Matterway.Customers.Api.Domain;
using Matterway.Customers.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Matterway.Customers.Api.Infrastructure.Persistence.EntityCustomer;

namespace Matterway.Customers.Api.Features.Customers;

public class CreateCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Customers", Handler)
            .WithName("CreateCustomer").WithSummary("Create a Customer.")
            .WithTags(nameof(Customer))
            .Produces<CustomerResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<CustomerResponse>, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(CustomerRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomerRepository customerRepository)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out ERequestClaimsRole userRole))
            return TypedResults.Forbid();

        var newEntity = MapToEntity(request);

        if (userRole == ERequestClaimsRole.Customer && newEntity.SystemUserId != systemUserId)
            return TypedResults.Forbid();

        newEntity.Id = newEntity.SystemUserId;

        var createdCustomer = await customerRepository.Create(newEntity);
        if (createdCustomer is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var location = linkGenerator.GetUriByName(httpContext, "GetCustomerById",
            new { id = createdCustomer.Id });

        return TypedResults.Created(location, MapToResponse(createdCustomer));
    }

    public record CustomerRequest
    {
        [Required]
        public Guid SystemUserId { get; init; }

        [Required]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        public string LastName { get; init; } = string.Empty;

        [Required]
        public DateOnly BirthDate { get; init; }

        public Guid? DefaultAddressId { get; init; }
    }

    public record CustomerResponse
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public Guid SystemUserId { get; init; }

        [Required]
        public string? FirstName { get; init; }

        [Required]
        public string? LastName { get; init; }

        [Required]
        public DateOnly BirthDate { get; init; }

        public Guid? DefaultAddressId { get; init; }
    }

    private static Customer MapToEntity(CustomerRequest request)
    {
        return new Customer
        {
            SystemUserId = request.SystemUserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            DefaultAddressId = request.DefaultAddressId
        };
    }

    private static CustomerResponse MapToResponse(Customer entity)
    {
        return new CustomerResponse
        {
            Id = entity.Id,
            SystemUserId = entity.SystemUserId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            DefaultAddressId = entity.DefaultAddressId
        };
    }
}