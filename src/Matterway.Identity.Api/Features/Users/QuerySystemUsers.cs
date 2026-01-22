using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Matterway.Identity.Api.Application;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Identity.Api.Features.Users;

public class QuerySystemUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("SystemUsers", Handler)
            .WithName("QuerySystemUsers").WithSummary("Query system users.")
            .WithTags("SystemUsers")
            .Produces<PaginationResponse<QuerySystemUsersResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<QuerySystemUsersResponse>>, NoContent,
            BadRequest<ProblemDetails>, ValidationProblem>>
        Handler([AsParameters] PaginationRequestParameters pagingQuery,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            UserManager<SystemUser> userManager)
    {
        var total = await userManager.Users.CountAsync();
        var entities = await userManager.Users
            .OrderBy(user => user.Created)
            .Skip((pagingQuery.Page - 1) * pagingQuery.PageSize)
            .Take(pagingQuery.PageSize)
            .ToListAsync();
        var baseUri = linkGenerator.GetPathByName(httpContext, "QuerySystemUsers");
        if (string.IsNullOrWhiteSpace(baseUri))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Unable to resolve pagination base URL."
            });

        var responseUsers = new List<QuerySystemUsersResponse>();
        foreach (var user in entities)
        {
            var role = await IdentityRoleAdapter.GetPrimaryRoleAsync(userManager, user);
            responseUsers.Add(new QuerySystemUsersResponse
            {
                Id = user.Id,
                Email = user.Email,
                Created = user.Created,
                Role = role
            });
        }

        var paginationResponse = PaginationResponse<QuerySystemUsersResponse>.Create(
            responseUsers,
            total,
            pagingQuery.Page,
            pagingQuery.PageSize,
            baseUri);

        return entities.Count > 0
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
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