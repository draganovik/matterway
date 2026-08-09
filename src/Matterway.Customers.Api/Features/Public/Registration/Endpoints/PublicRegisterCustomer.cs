using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Public.Registration.Contracts;
using Matterway.Customers.Api.Infrastructure.Brokers.Identity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Public.Registration.Endpoints;

public class PublicRegisterCustomer : IEndpoint
{
    private const string RouteName = nameof(PublicRegisterCustomer);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Public, "register", Handler)
            .WithName(RouteName)
            .WithSummary("[public] Register a new customer (creates identity user and customer profile).")
            .WithTags("Registration")
            .Produces<PublicRegisterCustomerResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .AllowAnonymous()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async
        Task<Results<Created<PublicRegisterCustomerResponse>, BadRequest<ProblemDetails>, ProblemHttpResult>>
        Handler(
            PublicRegisterCustomerRequest request,
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

        var created = await customerRepository.Create(new Customer
        {
            Id = identityResult.Data.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate
        }, cancellationToken);
        if (created is null)
        {
            var cleanupResult = await identityClient.DeleteCustomerUserAsync(identityResult.Data.Id, cancellationToken);
            if (!cleanupResult.IsSuccess)
                return TypedResults.Problem(new ProblemDetails
                {
                    Title = "Registration failed",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Could not clean up the identity user."
                });

            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Registration failed",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Could not create customer profile."
            });
        }

        var location = linkGenerator.GetUriByName(httpContext, "SelfGetProfile");
        return TypedResults.Created(location, new PublicRegisterCustomerResponse
        {
            SystemUserId = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            BirthDate = created.BirthDate
        });
    }
}