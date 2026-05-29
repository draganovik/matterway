using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Self.Customers.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Self.Customers.Endpoints;

public class SelfCreateCustomer : IEndpoint
{
    private const string RouteName = nameof(SelfCreateCustomer);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Self, "profile", Handler)
            .WithName(RouteName).WithSummary("[self] Create own Customer profile.")
            .WithTags(nameof(Customer))
            .Produces<SelfBaseCustomerResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<SelfBaseCustomerResponse>, BadRequest<ProblemDetails>, ForbidHttpResult>>
        Handler(SelfCreateCustomerRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomerRepository customerRepository,
            CancellationToken cancellationToken)
    {
        var newEntity = ToEntity(request);

        if (!RequestIdentity.CanManageOwnedResource(httpContext.User, newEntity.Id))
            return TypedResults.Forbid();

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

        var location = linkGenerator.GetUriByName(httpContext, "SelfGetProfile", null);

        return TypedResults.Created(location, ToResponse(createdCustomer));
    }

    private static Customer ToEntity(SelfCreateCustomerRequest request)
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

    private static SelfBaseCustomerResponse ToResponse(Customer entity)
    {
        return new SelfBaseCustomerResponse
        {
            SystemUserId = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            DefaultAddressId = entity.DefaultAddressId
        };
    }
}
