using Asp.Versioning;
using Matterway.Identity.Api.Application;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Matterway.Identity.Api.Features.UserClaims;

public class GetSystemUserClaims : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("SystemUsers/{id:guid}/claims", Handler)
            .WithName("GetSystemUserClaims")
            .WithSummary("Get claims for a system user.")
            .WithTags("SystemUsers")
            .Produces<IEnumerable<GetSystemUserClaimResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(EIdentityRole.Admin),
                nameof(EIdentityRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<IEnumerable<GetSystemUserClaimResponse>>, NotFound>> Handler(
        Guid id,
        UserManager<SystemUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var claims = await userManager.GetClaimsAsync(user);
        var response = claims.Select(claim => new GetSystemUserClaimResponse
        {
            Type = claim.Type,
            Value = claim.Value
        });

        return TypedResults.Ok(response);
    }

    public class GetSystemUserClaimResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}