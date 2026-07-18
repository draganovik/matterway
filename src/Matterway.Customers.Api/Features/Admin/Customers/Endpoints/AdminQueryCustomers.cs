using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.Admin.Customers.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;

namespace Matterway.Customers.Api.Features.Admin.Customers.Endpoints;

public class AdminQueryCustomers : IEndpoint
{
    private const string RouteName = nameof(AdminQueryCustomers);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "customers", Handler)
            .WithName(RouteName).WithSummary("[admin] Query Customers")
            .WithTags(nameof(Customer))
            .Produces<PaginationResponse<AdminBaseCustomerResponse>>()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Ok<PaginationResponse<AdminBaseCustomerResponse>>>
        Handler([AsParameters] PaginationRequestParameters pagingQuery,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomerRepository customerRepository,
            CancellationToken cancellationToken)
    {
        var total = await customerRepository.Count(cancellationToken);
        var entities = await customerRepository.Query(pagingQuery.Page, pagingQuery.PageSize, cancellationToken);
        var baseUri = linkGenerator.GetUriByName(httpContext, RouteName);

        var response = entities
            .Select(ToResponse)
            .ToList();

        var paginationResponse = PaginationResponse<AdminBaseCustomerResponse>.Create(
            response,
            total,
            pagingQuery.Page,
            pagingQuery.PageSize,
            baseUri);

        return TypedResults.Ok(paginationResponse);
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
