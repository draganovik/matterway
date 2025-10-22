using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using Customers.Api.Features.Customers.Contracts;
using Customers.Api.Features.Customers.Data;
using Customers.Api.Features.Customers.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Customers.Api.Features.Customers.Endpoints;

public class UpdateCustomerById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Customers/{id:guid}", Handler)
            .WithName("UpdateCustomerById").WithSummary("Update Customer by id.")
            .WithTags(nameof(Customer))
            .Produces<CustomerBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
            Results<Ok<CustomerBaseResponse>, NotFound, BadRequest<ProblemDetails>, ForbidHttpResult,
                ValidationProblem>>
        Handler(Guid id,
            CustomerBaseRequest request,
            HttpContext httpContext,
            ICustomerRepository customerRepository,
            IMapper mapper)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
            return TypedResults.Forbid();

        var updateEntity = mapper.Map<Customer>(request);
        updateEntity.Id = id;

        if (userRole == SystemUserRole.Customer && updateEntity.SystemUserId != systemUserId)
        {
            return TypedResults.Forbid();
        }

        var results = new List<ValidationResult>();
        var context = new ValidationContext(updateEntity);
        var isValid = Validator.TryValidateObject(updateEntity, context, results, true);

        if (!isValid)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred."
            };
            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var updatedUser = await customerRepository.Update(id, request);
        return updatedUser is not null
            ? TypedResults.Ok(mapper.Map<CustomerBaseResponse>(updatedUser))
            : TypedResults.NotFound();
    }
}