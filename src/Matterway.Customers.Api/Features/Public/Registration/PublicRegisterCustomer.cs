using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.ServiceDefaults.Api;
using Matterway.ServiceDefaults.Api;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Brokers.Identity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Customers.Api.Features.Public.Registration;

public class PublicRegisterCustomer : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Public, "register", Handler)
            .WithName("PublicRegisterCustomer")
            .WithSummary("[public] Register a new customer (creates identity user and customer profile).")
            .WithTags("Registration")
            .Produces<CustomerResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<CustomerResponse>, BadRequest<ProblemDetails>>> Handler(
        RegisterRequest request,
        IIdentityClient identityClient,
        ICustomerRepository customerRepository,
        LinkGenerator linkGenerator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var identityResult = await identityClient.CreateCustomerUserAsync(new CreateCustomerUserRequest
        {
            Email = request.Email,
            Password = request.Password
        }, cancellationToken);

        if (!identityResult.IsSuccess || identityResult.Data is null || identityResult.Data.Id == Guid.Empty)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Registration failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = identityResult.ErrorMessage ?? "Could not create identity user."
            });

        var customer = new Customer
        {
            Id = identityResult.Data.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            DefaultAddressId = null
        };

        var created = await customerRepository.Create(customer, cancellationToken);
        if (created is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Registration failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Could not create customer profile."
            });

        var location = linkGenerator.GetUriByName(httpContext, "SelfGetProfile", null);
        return TypedResults.Created(location, new CustomerResponse
        {
            SystemUserId = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            BirthDate = created.BirthDate
        });
    }

    public record RegisterRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; init; }

        [Required]
        [MinLength(6)]
        public required string Password { get; init; }

        [Required]
        public required string FirstName { get; init; }

        [Required]
        public required string LastName { get; init; }

        [Required]
        public DateOnly BirthDate { get; init; }
    }

    public record CustomerResponse
    {
        public Guid SystemUserId { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public DateOnly BirthDate { get; init; }
    }
}