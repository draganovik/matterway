using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Matterway.Identity.Api.Application;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Features.Users;

public class GetSystemUserById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("SystemUsers/{id:guid}", Handler)
            .WithName("GetSystemUserById").WithSummary("Get system user by id.")
            .WithTags("SystemUsers")
            .Produces<GetSystemUserByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<GetSystemUserByIdResponse>, NotFound>> Handler(
        Guid id,
        UserManager<SystemUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var role = await IdentityRoleAdapter.GetPrimaryRoleAsync(userManager, user);

        return TypedResults.Ok(new GetSystemUserByIdResponse
        {
            Id = user.Id,
            Email = user.Email,
            Created = user.Created,
            Role = role
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