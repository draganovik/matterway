using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Features.Orders.Contracts;
using Ordering.API.Features.Orders.Data;
using Ordering.API.Features.Orders.Domain;
using Ordering.API.Features.Shared;
using Common.Infrastructure.Interfaces;

namespace Ordering.API.Features.Orders.Endpoints;

public class CreateOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Orders", Handler)
            .WithName("CreateOrder").WithSummary("Create an Order.")
            .WithTags(nameof(Order))
            .Produces<OrderBaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<OrderBaseResponse>, BadRequest<ProblemDetails>, ValidationProblem>>
        Handler(OrderCreateRequest request,
            HttpContext httpContext,
            IOrderRepository orderRepository,
            IMapper mapper)
    {
        var newEntity = mapper.Map<Order>(request);
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

        newEntity = await orderRepository.Create(newEntity);
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

        var location = ResourceUrlHelper.BuildResourceLocation(httpContext, $"Orders/{newEntity.Id}");

        return TypedResults.Created(location, mapper.Map<OrderBaseResponse>(newEntity));
    }
}