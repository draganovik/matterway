using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Infrastructure.Persistence.SystemUserEntity;
using Microsoft.AspNetCore.WebUtilities;

namespace Matterway.Identity.Api.Endpoints.Admin.SystemUsers;

public class AdminQuerySystemUsers : IEndpoint
{
    private const string RouteName = nameof(AdminQuerySystemUsers);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "system-users", Handler)
            .WithName(RouteName).WithSummary("[admin] Query system users")
            .WithTags("SystemUsers")
            .Produces<PaginationResponse<QuerySystemUsersResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<PaginationResponse<QuerySystemUsersResponse>>, NoContent,
            BadRequest<ProblemDetails>, ValidationProblem>>
        Handler([AsParameters] QuerySystemUsersParameters queryParameters,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ISystemUserRepository systemUserRepository,
            CancellationToken cancellationToken)
    {
        if (!TryParseRoleFilter(queryParameters.Role, out var roleFilter))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Role must be one of: Customer, Employee."
            });

        var total = await systemUserRepository.Count(roleFilter, cancellationToken);
        if (total == 0) return TypedResults.NoContent();

        var entities = await systemUserRepository.Query(
            queryParameters.Page,
            queryParameters.PageSize,
            roleFilter,
            cancellationToken);

        var baseUri = linkGenerator.GetUriByName(httpContext, RouteName);
        if (string.IsNullOrWhiteSpace(baseUri))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Unable to resolve pagination base URL."
            });

        if (roleFilter.HasValue)
            baseUri = QueryHelpers.AddQueryString(baseUri, "Role", roleFilter.Value.ToString());

        var paginationResponse = PaginationResponse<QuerySystemUsersResponse>.Create(
            entities
                .Select(MapToResponse)
                .ToList(),
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            baseUri);

        return TypedResults.Ok(paginationResponse);
    }

    private static QuerySystemUsersResponse MapToResponse(SystemUserRepositoryModel user)
    {
        return new QuerySystemUsersResponse
        {
            Id = user.Id,
            Email = user.Email,
            Created = user.Created,
            Role = user.Role
        };
    }

    private static bool TryParseRoleFilter(string? rawRole, out EIdentityRole? role)
    {
        role = null;
        if (string.IsNullOrWhiteSpace(rawRole)) return true;

        var normalized = rawRole.Trim().ToLowerInvariant();
        role = normalized switch
        {
            "customer" or "customers" => EIdentityRole.Customer,
            "employee" or "employees" => EIdentityRole.Employee,
            _ => null
        };

        return role.HasValue;
    }

    public sealed record QuerySystemUsersParameters : PaginationRequestParameters
    {
        public string? Role { get; init; }
    }

    public class QuerySystemUsersResponse
    {
        public Guid Id { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public DateTime Created { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EIdentityRole Role { get; set; }
    }
}