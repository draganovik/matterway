using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Matterway.Customers.Api.Extensions;
using Matterway.Customers.Api.Features.Customers.Contracts;
using Matterway.Customers.Api.Features.Customers.Data;
using Matterway.Customers.Api.Features.Customers.Domain;

namespace Matterway.Customers.Api.Features.Customers.Endpoints;

public class CreateCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Customers", Handler)
            .WithName("CreateCustomer").WithSummary("Create a Customer.")
            .WithTags(nameof(Customer))
            .Produces<CustomerBaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
            Results<Created<CustomerBaseResponse>, BadRequest<ProblemDetails>, ForbidHttpResult, ValidationProblem>>
        Handler(CustomerBaseRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomerRepository customerRepository,
            IMapper mapper)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Forbid();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
            return TypedResults.Forbid();

        var newEntity = mapper.Map<Customer>(request);

        if (userRole == SystemUserRole.Customer && newEntity.SystemUserId != systemUserId) return TypedResults.Forbid();

        var results = new List<ValidationResult>();
        var context = new ValidationContext(newEntity);
        var isValid = Validator.TryValidateObject(newEntity, context, results, true);

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

        newEntity.Id = newEntity.SystemUserId;

        var createdCustomer = await customerRepository.Create(newEntity);
        if (createdCustomer is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var location = linkGenerator.GetPathByName(httpContext, "GetCustomerById",
            new { customerId = createdCustomer.Id });

        return TypedResults.Created(location,
            mapper.Map<CustomerBaseResponse>(createdCustomer));
    }
}