using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Features.OrderHistories.Contracts;
using Ordering.API.Features.OrderHistories.Data;
using Ordering.API.Features.OrderHistories.Domain;
using Ordering.API.Features.Shared;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Ordering.API.Features.OrderHistories.Endpoints;

public class CreateOrderHistory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("OrderHistories", Handler)
            .WithName("CreateOrderHistory").WithSummary("Create an Order History entry.")
            .WithTags(nameof(OrderHistory))
            .Produces<OrderHistoryBaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<OrderHistoryBaseResponse>, BadRequest<ProblemDetails>, ValidationProblem>>
        Handler(OrderHistoryBaseRequest request,
            HttpContext httpContext,
            IOrderHistoryRepository orderHistoryRepository,
            IMapper mapper)
    {
        var newEntity = mapper.Map<OrderHistory>(request);
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

        newEntity = await orderHistoryRepository.Create(newEntity);
        if (newEntity is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var location = ResourceUrlHelper.BuildResourceLocation(httpContext, $"OrderHistories/{newEntity.Id}");

        return TypedResults.Created(location, mapper.Map<OrderHistoryBaseResponse>(newEntity));
    }
}