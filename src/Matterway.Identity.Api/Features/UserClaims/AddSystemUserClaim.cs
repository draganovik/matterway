using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Asp.Versioning;
using Matterway.Identity.Api.Application;
using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Identity.Api.Features.UserClaims;

public class AddSystemUserClaim : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("SystemUsers/{id:guid}/claims", Handler)
            .WithName("AddSystemUserClaim")
            .WithSummary("Add a claim to a system user.")
            .WithTags("SystemUsers")
            .Produces<IEnumerable<AddSystemUserClaimResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(EIdentityRole.Admin)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async
        Task<Results<Ok<IEnumerable<AddSystemUserClaimResponse>>, BadRequest<ProblemDetails>, NotFound>>
        Handler(
            Guid id,
            AddSystemUserClaimRequest request,
            UserManager<SystemUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return TypedResults.NotFound();

        var claim = new Claim(request.Type!, request.Value!);
        var addResult = await userManager.AddClaimAsync(user, claim);
        if (!addResult.Succeeded)
            return TypedResults.BadRequest(CreateProblemDetails(string.Join("; ",
                addResult.Errors.Select(error => error.Description))));

        var claims = await userManager.GetClaimsAsync(user);
        var response = claims.Select(existing => new AddSystemUserClaimResponse
        {
            Type = existing.Type,
            Value = existing.Value
        });

        return TypedResults.Ok(response);
    }

    private static ProblemDetails CreateProblemDetails(string detail)
    {
        return new ProblemDetails
        {
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = detail
        };
    }

    public class AddSystemUserClaimRequest
    {
        [Required(ErrorMessage = "Claim Type is required.")]
        public string? Type { get; set; }

        [Required(ErrorMessage = "Claim Value is required.")]
        public string? Value { get; set; }
    }

    public class AddSystemUserClaimResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}