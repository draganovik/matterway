using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Services.Brokers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ordering.Api.Features.Orders.Contracts;
using Ordering.Api.Features.Orders.Data;
using Ordering.Api.Features.Orders.Domain;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Ordering.Api.Features.Orders.Endpoints;

public class UpdateOrderById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Orders/{id:guid}", Handler)
            .WithName("UpdateOrderById").WithSummary("Update Order by id.")
            .WithTags(nameof(Order))
            .Produces<OrderBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<
            Results<Ok<OrderBaseResponse>, NotFound, BadRequest<ProblemDetails>, UnauthorizedHttpResult,
                ForbidHttpResult>>
        Handler(Guid id,
            OrderUpdateRequest request,
            HttpContext httpContext,
            IOrderRepository orderRepository,
            ICustomersServiceBroker customerServiceBroker,
            IMapper mapper)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var systemUserId))
            return TypedResults.Unauthorized();

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
            return TypedResults.Unauthorized();

        if (request.CustomerId != null && userRole == SystemUserRole.Customer)
        {
            var customerId = await customerServiceBroker.VerifyBySystemUserId(systemUserId);
            if (customerId == null || customerId != request.CustomerId.Value) return TypedResults.Forbid();
        }
        else if (request.CustomerId != null &&
                 !await customerServiceBroker.VerifyByCustomerId(request.CustomerId.Value))
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Customer Id is not registrated in Customers API"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var results = new List<ValidationResult>();
        var context = new ValidationContext(request);
        var isValid = Validator.TryValidateObject(request, context, results, true);

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

        var updateEntity = await orderRepository.Update(id, request);
        return updateEntity is not null
            ? TypedResults.Ok(mapper.Map<OrderBaseResponse>(updateEntity))
            : TypedResults.NotFound();
    }
}