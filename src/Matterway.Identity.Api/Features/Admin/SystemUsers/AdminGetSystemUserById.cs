using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Infrastructure.Persistence.SystemUserEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Identity.Api.Features.Admin.SystemUsers;

public class AdminGetSystemUserById : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "system-users/{id:guid}", Handler)
            .WithName("AdminGetSystemUserById").WithSummary("[admin] Get system user by id")
            .WithTags("SystemUsers")
            .Produces<GetSystemUserByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<GetSystemUserByIdResponse>, NotFound>> Handler(
        Guid id,
        ISystemUserRepository systemUserRepository,
        CancellationToken cancellationToken)
    {
        var user = await systemUserRepository.GetById(id, cancellationToken);
        if (user is null) return TypedResults.NotFound();

        return TypedResults.Ok(new GetSystemUserByIdResponse
        {
            Id = user.Id,
            Email = user.Email,
            Created = user.Created,
            Role = user.Role
        });
    }

    public class GetSystemUserByIdResponse
    {
        public Guid Id { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public DateTime Created { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EIdentityRole Role { get; set; }
    }
}