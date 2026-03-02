using System.ComponentModel.DataAnnotations;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Endpoints.Admin.Customers;

public class AdminCreateCustomer : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "customers", Handler)
            .WithName("AdminCreateCustomer").WithSummary("[admin] Create a Customer")
            .WithTags(nameof(Customer))
            .Produces<CustomerResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<CustomerResponse>, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(CustomerRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomerRepository customerRepository,
            CancellationToken cancellationToken)
    {
        var newEntity = MapToEntity(request);

        var createdCustomer = await customerRepository.Create(newEntity, cancellationToken);
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

        var location = linkGenerator.GetUriByName(httpContext, "AdminGetCustomerById",
            new { customerId = createdCustomer.Id });

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
            Id = request.SystemUserId,
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
            SystemUserId = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            DefaultAddressId = entity.DefaultAddressId
        };
    }
}