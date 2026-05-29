using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Admin.Customers.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Admin.Customers.Endpoints;

public class AdminCreateCustomer : IEndpoint
{
    private const string RouteName = nameof(AdminCreateCustomer);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "customers", Handler)
            .WithName(RouteName).WithSummary("[admin] Create a Customer")
            .WithTags(nameof(Customer))
            .Produces<AdminBaseCustomerResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<AdminBaseCustomerResponse>, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(AdminCreateCustomerRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomerRepository customerRepository,
            CancellationToken cancellationToken)
    {
        var newEntity = ToEntity(request);

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

        return TypedResults.Created(location, ToResponse(createdCustomer));
    }

    private static Customer ToEntity(AdminCreateCustomerRequest request)
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

    private static AdminBaseCustomerResponse ToResponse(Customer entity)
    {
        return new AdminBaseCustomerResponse
        {
            SystemUserId = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            DefaultAddressId = entity.DefaultAddressId
        };
    }
}