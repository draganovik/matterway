using System.Text.Json.Serialization;
using Asp.Versioning;
using Matterway.Identity.Api.Application;
using Matterway.Identity.Api.Domain.Entities;
using Matterway.ServiceDefaults.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Features.UserPerms;

public class GetSystemUserPerms : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("SystemUsers/{id:guid}/perms", Handler)
            .WithName("GetSystemUserPerms")
            .WithSummary("Get permissions for a system user.")
            .WithTags("SystemUsers")
            .Produces<IEnumerable<GetSystemUserPermResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<IEnumerable<GetSystemUserPermResponse>>, NotFound>> Handler(
        Guid id,
        UserManager<SystemUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var claims = await userManager.GetClaimsAsync(user);
        var response = claims
            .Where(claim => string.Equals(claim.Type, PermissionClaims.ClaimType, StringComparison.Ordinal))
            .Select(claim => PermissionClaims.TryParse(claim.Value, out var service, out var level)
                ? new GetSystemUserPermResponse
                {
                    Service = service,
                    Level = level
                }
                : null)
            .Where(responseItem => responseItem is not null)
            .Select(responseItem => responseItem!);

        return TypedResults.Ok(response);
    }

    public class GetSystemUserPermResponse
    {
        public string Service { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PermissionLevel Level { get; set; }
    }
}